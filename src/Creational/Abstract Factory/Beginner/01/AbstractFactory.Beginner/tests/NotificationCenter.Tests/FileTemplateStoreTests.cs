using Microsoft.Extensions.Caching.Memory;
using NotificationCenter.Infrastructure.Templates;
using Xunit;

namespace NotificationCenter.Tests;

public sealed class FileTemplateStoreTests
{
    [Fact]
    public async Task ReadAsyncCachesContentAfterFirstRead()
    {
        var root = Path.Combine(Path.GetTempPath(), $"notification-template-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(root, "Email"));
        var filePath = Path.Combine(root, "Email", "welcome.html");
        await File.WriteAllTextAsync(filePath, "Hello {{CustomerName}}!");

        try
        {
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var store = new FileTemplateStore(root, cache, TimeSpan.FromMinutes(1));

            var first = await store.ReadAsync(Path.Combine("Email", "welcome.html"));
            var second = await store.ReadAsync(Path.Combine("Email", "welcome.html"));

            Assert.Equal("Hello {{CustomerName}}!", first);
            Assert.Equal(first, second);
            Assert.Equal(1, store.CacheMisses);
            Assert.Equal(1, store.CacheHits);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task ReadAsyncRejectsPathOutsideTemplateRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), $"notification-template-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);

        try
        {
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var store = new FileTemplateStore(root, cache);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => store.ReadAsync(Path.Combine("..", "outside.txt")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
