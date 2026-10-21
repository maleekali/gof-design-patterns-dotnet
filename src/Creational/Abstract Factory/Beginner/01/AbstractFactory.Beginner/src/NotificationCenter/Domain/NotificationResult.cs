namespace NotificationCenter.Domain;

public sealed record NotificationResult(
    NotificationChannel Channel,
    string Recipient,
    string RenderedContent);
