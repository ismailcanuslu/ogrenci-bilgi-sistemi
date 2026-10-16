using System.Collections.Concurrent;
using BuildingBlocks.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Api.Caching;

public sealed class MemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<CacheOptions> options,
    ILogger<MemoryCacheService>? logger = null)
    : ICacheService
{
    private readonly CacheOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, byte> _trackedKeys = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = BuildKey(key);

        if (memoryCache.TryGetValue(fullKey, out var val) && val is T typedVal)
        {
            return Task.FromResult<T?>(typedVal);
        }

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var fullKey = BuildKey(key);
        var exp = expiration ?? _options.DefaultExpiration;

        var entryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = exp
        };

        entryOptions.RegisterPostEvictionCallback((k, _, _, _) =>
        {
            _trackedKeys.TryRemove(k.ToString()!, out _);
        });

        memoryCache.Set(fullKey, value, entryOptions);
        _trackedKeys.TryAdd(fullKey, 0);

        logger?.LogDebug("Cached key {Key} for {Duration}", fullKey, exp);
        return Task.CompletedTask;
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

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = BuildKey(key);
        memoryCache.Remove(fullKey);
        _trackedKeys.TryRemove(fullKey, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var fullPrefix = BuildKey(prefix);
        var matchingKeys = _trackedKeys.Keys
            .Where(k => k.StartsWith(fullPrefix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in matchingKeys)
        {
            memoryCache.Remove(key);
            _trackedKeys.TryRemove(key, out _);
        }

        logger?.LogDebug("Evicted {Count} keys matching prefix {Prefix}", matchingKeys.Count, fullPrefix);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = BuildKey(key);
        return Task.FromResult(memoryCache.TryGetValue(fullKey, out _));
    }

    private string BuildKey(string key)
    {
        return string.IsNullOrWhiteSpace(_options.KeyPrefix)
            ? key
            : $"{_options.KeyPrefix}:{key}";
    }
}
