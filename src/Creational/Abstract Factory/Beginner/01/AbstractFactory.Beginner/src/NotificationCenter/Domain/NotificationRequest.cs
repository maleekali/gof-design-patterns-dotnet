namespace NotificationCenter.Domain;

public sealed record NotificationRequest(
    NotificationChannel Channel,
    string Recipient,
    string TemplateName,
    IReadOnlyDictionary<string, string> Values);
