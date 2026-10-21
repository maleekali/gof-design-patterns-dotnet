using FeedIngestion.Worker.Abstractions;
using FeedIngestion.Worker.Configuration;
using FeedIngestion.Worker.Domain;
using Microsoft.Extensions.Options;

namespace FeedIngestion.Worker.Files;

/// <summary>
/// Reads files from a local or mounted drop folder. Production deployments can replace this
/// adapter with an SFTP/S3 implementation without changing the ingestion pipeline.
/// </summary>
public sealed class DirectoryFileSource : IExternalFileSource
{
    private readonly string _incomingDirectory;
    private readonly string _processedDirectory;

    public DirectoryFileSource(IOptions<FeedOptions> options)
    {
        _incomingDirectory = Path.GetFullPath(options.Value.IncomingDirectory);
        _processedDirectory = Path.GetFullPath(options.Value.ProcessedDirectory);
        Directory.CreateDirectory(_incomingDirectory);
        Directory.CreateDirectory(_processedDirectory);
    }

    public Task<IReadOnlyList<ExternalFeedFile>> ListPendingFilesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<ExternalFeedFile> files = Directory
            .EnumerateFiles(_incomingDirectory, "*.jsonl", SearchOption.TopDirectoryOnly)
            .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
            .Select(path => new ExternalFeedFile(Path.GetFullPath(path), Path.GetFileName(path)))
            .ToArray();

        return Task.FromResult(files);
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

    public Task ArchiveAsync(ExternalFeedFile file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(_processedDirectory);
        var destination = Path.Combine(_processedDirectory, file.Name);
        if (File.Exists(destination))
        {
            var stem = Path.GetFileNameWithoutExtension(file.Name);
            var extension = Path.GetExtension(file.Name);
            destination = Path.Combine(
                _processedDirectory,
                $"{stem}.{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}{extension}");
        }

        File.Move(file.FullPath, destination, overwrite: false);
        return Task.CompletedTask;
    }
}
