using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NotificationCenter.Abstractions;
using NotificationCenter.Application;
using NotificationCenter.Domain;
using NotificationCenter.Infrastructure.Factories;
using NotificationCenter.Infrastructure.Templates;

var services = new ServiceCollection();
services.AddMemoryCache();

services.AddSingleton(serviceProvider =>
    new FileTemplateStore(
        Path.Combine(AppContext.BaseDirectory, "Templates"),
        serviceProvider.GetRequiredService<IMemoryCache>()));

services.AddSingleton<INotificationFactory, EmailNotificationFactory>();
services.AddSingleton<INotificationFactory, SmsNotificationFactory>();
services.AddTransient<NotificationService>();

using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();

var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
var values = new Dictionary<string, string>(StringComparer.Ordinal)
{
    ["CustomerName"] = "Alex Morgan",
    ["OrderNumber"] = "ORD-1042"
};

Console.WriteLine("=== First email ===");
await notificationService.SendAsync(new NotificationRequest(
    NotificationChannel.Email,
    "alex@example.com",
    "order-created",
    values));

Console.WriteLine();
Console.WriteLine("=== Same email template again (cache hit) ===");
await notificationService.SendAsync(new NotificationRequest(
    NotificationChannel.Email,
    "alex@example.com",
    "order-created",
    values));

Console.WriteLine();
Console.WriteLine("=== SMS ===");
await notificationService.SendAsync(new NotificationRequest(
    NotificationChannel.Sms,
    "+1-555-0100",
    "order-created",
    values));

var templateStore = serviceProvider.GetRequiredService<FileTemplateStore>();
Console.WriteLine();
Console.WriteLine($"Template cache: {templateStore.CacheHits} hit(s), {templateStore.CacheMisses} miss(es).");
Console.WriteLine("Note: senders are simulated; no external message was sent.");
