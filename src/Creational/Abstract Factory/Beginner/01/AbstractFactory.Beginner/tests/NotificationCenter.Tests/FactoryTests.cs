using Microsoft.Extensions.Caching.Memory;
using NotificationCenter.Abstractions;
using NotificationCenter.Domain;
using NotificationCenter.Infrastructure.Factories;
using NotificationCenter.Infrastructure.Renderers;
using NotificationCenter.Infrastructure.Senders;
using NotificationCenter.Infrastructure.Templates;
using Xunit;

namespace NotificationCenter.Tests;

public sealed class FactoryTests
{
    [Fact]
    public void EmailFactoryCreatesProductsFromEmailFamily()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = new FileTemplateStore(Path.GetTempPath(), cache);
        INotificationFactory factory = new EmailNotificationFactory(store);

        Assert.Equal(NotificationChannel.Email, factory.Channel);
        Assert.IsType<ConsoleEmailSender>(factory.CreateSender());
        Assert.IsType<EmailTemplateRenderer>(factory.CreateTemplateRenderer());
    }

    [Fact]
    public void SmsFactoryCreatesProductsFromSmsFamily()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = new FileTemplateStore(Path.GetTempPath(), cache);
        INotificationFactory factory = new SmsNotificationFactory(store);

        Assert.Equal(NotificationChannel.Sms, factory.Channel);
        Assert.IsType<ConsoleSmsSender>(factory.CreateSender());
        Assert.IsType<SmsTemplateRenderer>(factory.CreateTemplateRenderer());
    }
}
