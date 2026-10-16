using System.Collections.Concurrent;

namespace BuildingBlocks.Api.Idempotency;

public sealed class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly ConcurrentDictionary<string, IdempotencyEntry> _entries = new();

    public Task<IdempotencyEntry?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        _entries.TryGetValue(key, out var entry);
        return Task.FromResult(entry);
    }

    public Task<bool> TryCreateAsync(IdempotencyEntry entry, CancellationToken cancellationToken = default)
    {
        var added = _entries.TryAdd(entry.Key, entry);
        return Task.FromResult(added);
    }
}
