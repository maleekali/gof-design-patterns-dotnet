using FeedIngestion.Worker.Abstractions;
using FeedIngestion.Worker.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace FeedIngestion.Worker.State;

public sealed class RedisProcessingStateStore : IProcessingStateStore
{
    private const string ReleaseLeaseScript = "if redis.call('get', KEYS[1]) == ARGV[1] then return redis.call('del', KEYS[1]) else return 0 end";
    private readonly IDatabase _database;
    private readonly FeedOptions _options;

    public RedisProcessingStateStore(IConnectionMultiplexer connection, IOptions<FeedOptions> options)
    {
        _database = connection.GetDatabase();
        _options = options.Value;
    }

    public async Task<bool> IsFileCompletedAsync(string fileHash, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _database.KeyExistsAsync(CompletedKey(fileHash));
    }

    public async Task<bool> TryAcquireFileLeaseAsync(
        string fileHash, string ownerToken, TimeSpan duration, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _database.StringSetAsync(LeaseKey(fileHash), ownerToken, duration, When.NotExists);
    }

    public async Task ReleaseFileLeaseAsync(string fileHash, string ownerToken, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _database.ScriptEvaluateAsync(
            ReleaseLeaseScript,
            new RedisKey[] { LeaseKey(fileHash) },
            new RedisValue[] { ownerToken });
    }

    public async Task<bool> IsRecordProcessedAsync(string fileHash, long lineNumber, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _database.KeyExistsAsync(RecordKey(fileHash, lineNumber));
    }

    public async Task MarkRecordProcessedAsync(string fileHash, long lineNumber, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _database.StringSetAsync(
            RecordKey(fileHash, lineNumber), "1", TimeSpan.FromDays(Math.Max(1, _options.RecordMarkerDays)));
    }

    public async Task MarkFileCompletedAsync(string fileHash, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _database.StringSetAsync(
            CompletedKey(fileHash), "1", TimeSpan.FromDays(Math.Max(1, _options.CompletedFileDays)));
    }

    private static string CompletedKey(string hash) => $"feed:file:{hash}:completed";
    private static string LeaseKey(string hash) => $"feed:file:{hash}:lease";
    private static string RecordKey(string hash, long line) => $"feed:record:{hash}:{line}:processed";
}
