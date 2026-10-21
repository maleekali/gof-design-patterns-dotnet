using System.Text.RegularExpressions;
using FeedIngestion.Worker.Abstractions;
using FeedIngestion.Worker.Configuration;
using FeedIngestion.Worker.Domain;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace FeedIngestion.Worker.Files;

/// <summary>
/// Optional SFTP adapter. Files are downloaded to a local spool so the pipeline can hash and
/// stream them through the same IExternalFileSource contract used by the directory adapter.
/// </summary>
public sealed class SftpExternalFileSource : IExternalFileSource
{
    private readonly SftpOptions _options;
    private readonly string _spoolDirectory;

    public SftpExternalFileSource(IOptions<SftpOptions> options)
    {
        _options = options.Value;
        _spoolDirectory = Path.GetFullPath(_options.LocalSpoolDirectory);
        Directory.CreateDirectory(_spoolDirectory);
    }

    public async Task<IReadOnlyList<ExternalFeedFile>> ListPendingFilesAsync(CancellationToken cancellationToken)
    {
        using var client = CreateClient();
        await client.ConnectAsync(cancellationToken);

        var pending = new List<ExternalFeedFile>();
        await foreach (var remoteFile in client.ListDirectoryAsync(_options.IncomingDirectory, cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (remoteFile.IsDirectory || remoteFile.IsSymbolicLink ||
                !remoteFile.Name.EndsWith(".jsonl", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var safeName = Regex.Replace(remoteFile.Name, "[^a-zA-Z0-9._-]", "_");
            var versionedName = $"{safeName}.{remoteFile.Length}.{remoteFile.LastWriteTimeUtc.Ticks}.spool";
            var localPath = Path.Combine(_spoolDirectory, versionedName);
            if (!File.Exists(localPath))
            {
                var temporaryPath = localPath + ".downloading";
                try
                {
                    await using (var output = new FileStream(
                        temporaryPath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 64 * 1024,
                        options: FileOptions.Asynchronous | FileOptions.SequentialScan))
                    {
                        await client.DownloadFileAsync(remoteFile.FullName, output, cancellationToken);
                        await output.FlushAsync(cancellationToken);
                    }

                    File.Move(temporaryPath, localPath, overwrite: true);
                }
                catch
                {
                    if (File.Exists(temporaryPath))
                        File.Delete(temporaryPath);
                    throw;
                }
            }

            pending.Add(new ExternalFeedFile(localPath, remoteFile.Name, remoteFile.FullName));
        }

        return pending.OrderBy(file => file.Name, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public Task<Stream> OpenReadAsync(ExternalFeedFile file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Stream stream = new FileStream(
            file.FullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 64 * 1024,
            options: FileOptions.Asynchronous | FileOptions.SequentialScan);
        return Task.FromResult(stream);
    }

    public async Task ArchiveAsync(ExternalFeedFile file, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(file.RemotePath))
        {
            using var client = CreateClient();
            await client.ConnectAsync(cancellationToken);

            if (!await client.ExistsAsync(_options.ProcessedDirectory, cancellationToken))
                await client.CreateDirectoryAsync(_options.ProcessedDirectory, cancellationToken);

            var destination = CombineRemotePath(_options.ProcessedDirectory, file.Name);
            if (await client.ExistsAsync(destination, cancellationToken))
            {
                var stem = Path.GetFileNameWithoutExtension(file.Name);
                var extension = Path.GetExtension(file.Name);
                destination = CombineRemotePath(
                    _options.ProcessedDirectory,
                    $"{stem}.{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}{extension}");
            }

            await client.RenameFileAsync(file.RemotePath, destination, cancellationToken);
        }

        if (File.Exists(file.FullPath))
            File.Delete(file.FullPath);
    }

    private SftpClient CreateClient() => new(
        _options.Host,
        _options.Port,
        _options.UserName,
        _options.Password);

    private static string CombineRemotePath(string directory, string fileName)
        => $"{directory.TrimEnd('/')}/{fileName}";
}
