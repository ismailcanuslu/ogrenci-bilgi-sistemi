using System.Collections.Concurrent;
using System.Text.Json;
using BuildingBlocks.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Api.Caching;

public sealed class DistributedCacheService(
    IDistributedCache distributedCache,
    IOptions<CacheOptions> options,
    ILogger<DistributedCacheService>? logger = null)
    : ICacheService
{
    private readonly CacheOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, byte> _trackedKeys = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = BuildKey(key);
        var bytes = await distributedCache.GetAsync(fullKey, cancellationToken);
        if (bytes is null || bytes.Length == 0)
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(bytes, JsonOptions);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to deserialize cached value for key {Key}", fullKey);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var fullKey = BuildKey(key);
        var exp = expiration ?? _options.DefaultExpiration;

        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);

        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = exp
        };

        await distributedCache.SetAsync(fullKey, bytes, entryOptions, cancellationToken);
        _trackedKeys.TryAdd(fullKey, 0);

        logger?.LogDebug("Distributed-cached key {Key} for {Duration}", fullKey, exp);
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync<T>(key, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var created = await factory(cancellationToken);
        await SetAsync(key, created, expiration, cancellationToken);
        return created;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = BuildKey(key);
        await distributedCache.RemoveAsync(fullKey, cancellationToken);
        _trackedKeys.TryRemove(fullKey, out _);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var fullPrefix = BuildKey(prefix);
        var matchingKeys = _trackedKeys.Keys
            .Where(k => k.StartsWith(fullPrefix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in matchingKeys)
        {
            await distributedCache.RemoveAsync(key, cancellationToken);
            _trackedKeys.TryRemove(key, out _);
        }

        logger?.LogDebug("Evicted {Count} distributed keys matching prefix {Prefix}", matchingKeys.Count, fullPrefix);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = BuildKey(key);
        var bytes = await distributedCache.GetAsync(fullKey, cancellationToken);
        return bytes is not null && bytes.Length > 0;
    }

    private string BuildKey(string key)
    {
        return string.IsNullOrWhiteSpace(_options.KeyPrefix)
            ? key
            : $"{_options.KeyPrefix}:{key}";
    }
}
