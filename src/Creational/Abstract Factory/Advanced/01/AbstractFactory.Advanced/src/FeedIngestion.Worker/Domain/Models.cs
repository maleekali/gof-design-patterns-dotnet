namespace FeedIngestion.Worker.Domain;

public sealed class OrderFeedRecord
{
    public string? OrderId { get; init; }
    public string? CustomerEmail { get; init; }
    public decimal Amount { get; init; }
    public string? Currency { get; init; }
}

public sealed record OrderImportedEvent(
    string EventId,
    string OrderId,
    string CustomerEmail,
    decimal Amount,
    string Currency,
    DateTimeOffset OccurredAtUtc,
    string SourceFileHash,
    long LineNumber);

public sealed record FeedFailure(
    string EventId,
    string SourceFileHash,
    string FileName,
    long LineNumber,
    string Reason,
    string RawRecord,
    DateTimeOffset OccurredAtUtc);

public sealed record ExternalFeedFile(string FullPath, string Name, string? RemotePath = null);

public sealed record OutboundMessage(
    string Destination,
    string Key,
    string MessageId,
    byte[] Body,
    string ContentType = "application/json");
