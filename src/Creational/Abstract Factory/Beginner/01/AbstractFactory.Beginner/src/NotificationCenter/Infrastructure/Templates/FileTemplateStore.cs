using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace NotificationCenter.Infrastructure.Templates;

public sealed class FileTemplateStore
{
    private readonly string _rootDirectory;
    private readonly string _rootPrefix;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration;
    private long _cacheHits;
    private long _cacheMisses;

    public FileTemplateStore(
        string rootDirectory,
        IMemoryCache cache,
        TimeSpan? cacheDuration = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);
        ArgumentNullException.ThrowIfNull(cache);

        _rootDirectory = Path.GetFullPath(rootDirectory);
        _rootPrefix = _rootDirectory.EndsWith(Path.DirectorySeparatorChar)
            ? _rootDirectory
            : _rootDirectory + Path.DirectorySeparatorChar;
        _cache = cache;
        _cacheDuration = cacheDuration ?? TimeSpan.FromMinutes(5);

        if (_cacheDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(cacheDuration), "Cache duration must be positive.");
        }
    }

    public long CacheHits => Interlocked.Read(ref _cacheHits);

    public long CacheMisses => Interlocked.Read(ref _cacheMisses);

    public async Task<string> ReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        var fullPath = Path.GetFullPath(Path.Combine(_rootDirectory, relativePath));
        var pathComparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        if (!fullPath.StartsWith(_rootPrefix, pathComparison))
        {
            throw new InvalidOperationException("The requested template path is outside the template directory.");
        }

        var cacheKey = $"notification-template:{Path.GetRelativePath(_rootDirectory, fullPath).Replace(Path.DirectorySeparatorChar, '/')}";
        if (_cache.TryGetValue(cacheKey, out string? cachedContent) && cachedContent is not null)
        {
            Interlocked.Increment(ref _cacheHits);
            return cachedContent;
        }

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Template file was not found: '{relativePath}'.", fullPath);
        }

        var content = await File.ReadAllTextAsync(fullPath, cancellationToken);
        _cache.Set(
            cacheKey,
            content,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration));
        Interlocked.Increment(ref _cacheMisses);
        return content;
    }
}
