using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using FeedIngestion.Worker.Abstractions;
using FeedIngestion.Worker.Configuration;
using FeedIngestion.Worker.Domain;
using Microsoft.Extensions.Options;

namespace FeedIngestion.Worker.Processing;

public sealed class FeedIngestionWorker : BackgroundService
{
    private const int MaxRawRecordLength = 2048;
    private readonly IExternalFileSource _fileSource;
    private readonly IProcessingStateStore _stateStore;
    private readonly IMessageBrokerFactory _factory;
    private readonly IOrderFeedRecordParser _parser;
    private readonly FeedOptions _options;
    private readonly ILogger<FeedIngestionWorker> _logger;

    public FeedIngestionWorker(
        IExternalFileSource fileSource,
        IProcessingStateStore stateStore,
        IMessageBrokerFactory factory,
        IOrderFeedRecordParser parser,
        IOptions<FeedOptions> options,
        ILogger<FeedIngestionWorker> logger)
    {
        _fileSource = fileSource;
        _stateStore = stateStore;
        _factory = factory;
        _parser = parser;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var publisher = _factory.CreatePublisher();
        var encoder = _factory.CreateEventEncoder();
        var failurePublisher = _factory.CreateFailurePublisher();
        var interval = TimeSpan.FromSeconds(Math.Max(1, _options.PollIntervalSeconds));

        _logger.LogInformation("Feed ingestion started using {FactoryType}.", _factory.GetType().Name);

        while (!stoppingToken.IsCancellationRequested)
        {
            IReadOnlyList<ExternalFeedFile> files;
            try
            {
                files = await _fileSource.ListPendingFilesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Could not list incoming feed files.");
                await Task.Delay(interval, stoppingToken);
                continue;
            }

            foreach (var file in files)
            {
                stoppingToken.ThrowIfCancellationRequested();
                try
                {
                    await ProcessFileAsync(file, publisher, encoder, failurePublisher, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Processing failed for file {FileName}; it remains eligible for retry.", file.Name);
                }
            }

            await Task.Delay(interval, stoppingToken);
        }
    }

    private async Task ProcessFileAsync(
        ExternalFeedFile file,
        IMessagePublisher publisher,
        IBrokerMessageEncoder encoder,
        IFailurePublisher failurePublisher,
        CancellationToken cancellationToken)
    {
        var fileHash = await CalculateFileHashAsync(file, cancellationToken);
        if (await _stateStore.IsFileCompletedAsync(fileHash, cancellationToken))
        {
            await _fileSource.ArchiveAsync(file, cancellationToken);
            _logger.LogInformation("Archived previously completed duplicate file {FileName}.", file.Name);
            return;
        }

        var ownerToken = Guid.NewGuid().ToString("N");
        var lease = TimeSpan.FromSeconds(Math.Max(30, _options.LeaseSeconds));
        if (!await _stateStore.TryAcquireFileLeaseAsync(fileHash, ownerToken, lease, cancellationToken))
        {
            _logger.LogInformation("Skipping {FileName}; another worker owns its Redis lease.", file.Name);
            return;
        }

        try
        {
            if (await _stateStore.IsFileCompletedAsync(fileHash, cancellationToken))
            {
                await _fileSource.ArchiveAsync(file, cancellationToken);
                return;
            }

            await using var stream = await _fileSource.OpenReadAsync(file, cancellationToken);
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            long lineNumber = 0;
            long publishedCount = 0;
            long rejectedCount = 0;

            while (await reader.ReadLineAsync(cancellationToken) is { } line)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (await _stateStore.IsRecordProcessedAsync(fileHash, lineNumber, cancellationToken))
                    continue;

                if (!_parser.TryParse(line, out var record, out var validationError) || record is null)
                {
                    var failure = new FeedFailure(
                        EventId: CreateEventId("failure", fileHash, lineNumber),
                        SourceFileHash: fileHash,
                        FileName: file.Name,
                        LineNumber: lineNumber,
                        Reason: validationError,
                        RawRecord: Truncate(line, MaxRawRecordLength),
                        OccurredAtUtc: DateTimeOffset.UtcNow);

                    await failurePublisher.PublishAsync(failure, cancellationToken);
                    await _stateStore.MarkRecordProcessedAsync(fileHash, lineNumber, cancellationToken);
                    rejectedCount++;
                    _logger.LogWarning("Rejected record at {FileName}:{LineNumber}: {Reason}", file.Name, lineNumber, validationError);
                    continue;
                }

                var imported = new OrderImportedEvent(
                    EventId: CreateEventId("order-imported", fileHash, lineNumber),
                    OrderId: record.OrderId!,
                    CustomerEmail: record.CustomerEmail!,
                    Amount: record.Amount,
                    Currency: record.Currency!,
                    OccurredAtUtc: DateTimeOffset.UtcNow,
                    SourceFileHash: fileHash,
                    LineNumber: lineNumber);

                await publisher.PublishAsync(encoder.Encode(imported), cancellationToken);
                // Mark only after the broker publish returns successfully: this is at-least-once,
                // so a crash between publish and this write may result in a duplicate.
                await _stateStore.MarkRecordProcessedAsync(fileHash, lineNumber, cancellationToken);
                publishedCount++;
            }

            await _stateStore.MarkFileCompletedAsync(fileHash, cancellationToken);
            await _fileSource.ArchiveAsync(file, cancellationToken);
            _logger.LogInformation(
                "Completed {FileName}. Published={PublishedCount}, rejected={RejectedCount}, hash={FileHash}.",
                file.Name, publishedCount, rejectedCount, fileHash);
        }
        finally
        {
            // Release should still be attempted during shutdown; Redis lease expiry is the fallback.
            try
            {
                await _stateStore.ReleaseFileLeaseAsync(fileHash, ownerToken, CancellationToken.None);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Could not release lease for file hash {FileHash}; it will expire automatically.", fileHash);
            }
        }
    }

    private async Task<string> CalculateFileHashAsync(ExternalFeedFile file, CancellationToken cancellationToken)
    {
        await using var stream = await _fileSource.OpenReadAsync(file, cancellationToken);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string CreateEventId(string prefix, string fileHash, long lineNumber)
        => $"{prefix}:{fileHash}:{lineNumber}";

    private static string Truncate(string value, int maxLength)
        => value.Length <= maxLength ? value : value[..maxLength];
}
