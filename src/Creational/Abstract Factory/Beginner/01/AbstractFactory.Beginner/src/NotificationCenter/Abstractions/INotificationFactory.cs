using NotificationCenter.Domain;

namespace NotificationCenter.Abstractions;

public interface INotificationFactory
{
    NotificationChannel Channel { get; }

    INotificationSender CreateSender();

    ITemplateRenderer CreateTemplateRenderer();
}
