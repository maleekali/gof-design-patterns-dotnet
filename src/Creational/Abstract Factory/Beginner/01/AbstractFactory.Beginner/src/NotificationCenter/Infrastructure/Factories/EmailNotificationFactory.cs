using NotificationCenter.Abstractions;
using NotificationCenter.Domain;
using NotificationCenter.Infrastructure.Renderers;
using NotificationCenter.Infrastructure.Senders;
using NotificationCenter.Infrastructure.Templates;

namespace NotificationCenter.Infrastructure.Factories;

public sealed class EmailNotificationFactory(FileTemplateStore templateStore) : INotificationFactory
{
    public NotificationChannel Channel => NotificationChannel.Email;

    public INotificationSender CreateSender() => new ConsoleEmailSender();

    public ITemplateRenderer CreateTemplateRenderer() => new EmailTemplateRenderer(templateStore);
}
