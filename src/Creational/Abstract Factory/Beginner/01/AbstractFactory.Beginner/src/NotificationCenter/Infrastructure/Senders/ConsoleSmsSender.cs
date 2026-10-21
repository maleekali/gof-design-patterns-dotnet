using NotificationCenter.Abstractions;

namespace NotificationCenter.Infrastructure.Senders;

public sealed class ConsoleSmsSender : INotificationSender
{
    public Task SendAsync(
        string recipient,
        string content,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine($"[SIMULATED SMS] To: {recipient}");
        Console.WriteLine(content);
        return Task.CompletedTask;
    }
}
