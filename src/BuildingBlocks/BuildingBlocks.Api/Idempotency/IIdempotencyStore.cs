namespace BuildingBlocks.Api.Idempotency;

public interface IIdempotencyStore
{
    Task<IdempotencyEntry?> GetAsync(
        string key,
        CancellationToken cancellationToken = default);

    Task<bool> TryCreateAsync(
        IdempotencyEntry entry,
        CancellationToken cancellationToken = default);
}

public sealed record IdempotencyEntry(
    string Key,
    int StatusCode,
    string? ContentType,
    byte[] ResponseBody);