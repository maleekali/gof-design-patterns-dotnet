using NotificationCenter.Abstractions;
using NotificationCenter.Domain;

namespace NotificationCenter.Application;

public sealed class NotificationService
{
    private readonly IReadOnlyDictionary<NotificationChannel, INotificationFactory> _factories;

    public NotificationService(IEnumerable<INotificationFactory> factories)
    {
        ArgumentNullException.ThrowIfNull(factories);

        var factoryList = factories.ToList();
        var duplicateChannel = factoryList
            .GroupBy(factory => factory.Channel)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateChannel is not null)
        {
            throw new InvalidOperationException(
                $"More than one notification factory is registered for channel '{duplicateChannel.Key}'.");
        }

        _factories = factoryList.ToDictionary(factory => factory.Channel);
    }

    public async Task<NotificationResult> SendAsync(
        NotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Recipient);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TemplateName);
        ArgumentNullException.ThrowIfNull(request.Values);

        if (!_factories.TryGetValue(request.Channel, out var factory))
        {
            throw new NotSupportedException(
                $"No notification factory is registered for channel '{request.Channel}'.");
        }

        var renderer = factory.CreateTemplateRenderer();
        var sender = factory.CreateSender();
        var content = await renderer.RenderAsync(
            request.TemplateName,
            request.Values,
            cancellationToken);

        await sender.SendAsync(request.Recipient, content, cancellationToken);

        return new NotificationResult(request.Channel, request.Recipient, content);
    }
}
