using System.Text.Json;
using ImportPreview.Api.Contracts;
using StackExchange.Redis;

namespace ImportPreview.Api.Caching;

public sealed class RedisImportPreviewCache : IImportPreviewCache
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IConnectionMultiplexer _connection;
    private readonly ILogger<RedisImportPreviewCache> _logger;
    private readonly TimeSpan _timeToLive;

    public RedisImportPreviewCache(
        IConnectionMultiplexer connection,
        ILogger<RedisImportPreviewCache> logger,
        TimeSpan timeToLive)
    {
        _connection = connection;
        _logger = logger;
        _timeToLive = timeToLive;
    }

    public async Task<ImportPreviewResponse?> GetAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            var value = await _connection.GetDatabase().StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<ImportPreviewResponse>(value.ToString(), SerializerOptions);
        }
        catch (Exception exception) when (exception is RedisException or TimeoutException or JsonException)
        {
            _logger.LogWarning(exception, "Redis cache read failed for key {CacheKey}; processing will continue without a cache hit.", key);
            return null;
        }
    }

    public async Task SetAsync(string key, ImportPreviewResponse value, CancellationToken cancellationToken)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(value, SerializerOptions);
            await _connection.GetDatabase().StringSetAsync(key, serialized, _timeToLive);
        }
        catch (Exception exception) when (exception is RedisException or TimeoutException or JsonException)
        {
            _logger.LogWarning(exception, "Redis cache write failed for key {CacheKey}; the preview is still returned.", key);
        }
    }
}
