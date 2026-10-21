using NotificationCenter.Abstractions;
using NotificationCenter.Domain;
using NotificationCenter.Infrastructure.Renderers;
using NotificationCenter.Infrastructure.Senders;
using NotificationCenter.Infrastructure.Templates;

namespace NotificationCenter.Infrastructure.Factories;

public sealed class SmsNotificationFactory(FileTemplateStore templateStore) : INotificationFactory
{
    public NotificationChannel Channel => NotificationChannel.Sms;

    public INotificationSender CreateSender() => new ConsoleSmsSender();

    public ITemplateRenderer CreateTemplateRenderer() => new SmsTemplateRenderer(templateStore);
}
