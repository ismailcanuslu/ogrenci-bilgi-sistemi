namespace BuildingBlocks.Application.Abstractions.Caching;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public string Provider { get; set; } = "Memory"; // "Memory" or "Redis"

    public string? RedisConnectionString { get; set; }

    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);

    public string KeyPrefix { get; set; } = "obs";
}
