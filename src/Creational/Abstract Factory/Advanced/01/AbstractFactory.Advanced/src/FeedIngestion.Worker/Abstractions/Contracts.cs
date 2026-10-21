using FeedIngestion.Worker.Domain;

namespace FeedIngestion.Worker.Abstractions;

public interface IMessageBrokerFactory
{
    IMessagePublisher CreatePublisher();
    IBrokerMessageEncoder CreateEventEncoder();
    IFailurePublisher CreateFailurePublisher();
}

public interface IMessagePublisher
{
    Task PublishAsync(OutboundMessage message, CancellationToken cancellationToken);
}

public interface IBrokerMessageEncoder
{
    OutboundMessage Encode(OrderImportedEvent message);
}

public interface IFailurePublisher
{
    Task PublishAsync(FeedFailure failure, CancellationToken cancellationToken);
}

public interface IBrokerTransport : IAsyncDisposable
{
    Task PublishAsync(OutboundMessage message, CancellationToken cancellationToken);
}

public interface IProcessingStateStore
{
    Task<bool> IsFileCompletedAsync(string fileHash, CancellationToken cancellationToken);
    Task<bool> TryAcquireFileLeaseAsync(string fileHash, string ownerToken, TimeSpan duration, CancellationToken cancellationToken);
    Task ReleaseFileLeaseAsync(string fileHash, string ownerToken, CancellationToken cancellationToken);
    Task<bool> IsRecordProcessedAsync(string fileHash, long lineNumber, CancellationToken cancellationToken);
    Task MarkRecordProcessedAsync(string fileHash, long lineNumber, CancellationToken cancellationToken);
    Task MarkFileCompletedAsync(string fileHash, CancellationToken cancellationToken);
}

public interface IExternalFileSource
{
    Task<IReadOnlyList<ExternalFeedFile>> ListPendingFilesAsync(CancellationToken cancellationToken);
    Task<Stream> OpenReadAsync(ExternalFeedFile file, CancellationToken cancellationToken);
    Task ArchiveAsync(ExternalFeedFile file, CancellationToken cancellationToken);
}

public interface IOrderFeedRecordParser
{
    bool TryParse(string jsonLine, out OrderFeedRecord? record, out string error);
}
