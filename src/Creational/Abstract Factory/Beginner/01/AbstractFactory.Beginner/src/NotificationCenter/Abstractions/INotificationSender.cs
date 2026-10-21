namespace NotificationCenter.Abstractions;

public interface INotificationSender
{
    Task SendAsync(
        string recipient,
        string content,
        CancellationToken cancellationToken = default);
}
