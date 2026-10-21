using NotificationCenter.Abstractions;

namespace NotificationCenter.Infrastructure.Senders;

public sealed class ConsoleEmailSender : INotificationSender
{
    public Task SendAsync(
        string recipient,
        string content,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine($"[SIMULATED EMAIL] To: {recipient}");
        Console.WriteLine(content);
        return Task.CompletedTask;
    }
}
