using BuildingBlocks.Api.Caching;
using BuildingBlocks.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace UniversalSystem.UnitTests.Caching;

public class CacheServiceTests
{
    private sealed record TestUser(Guid Id, string Name);

    [Fact]
    public async Task MemoryCacheService_SetAndGet_ShouldReturnStoredValue()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new CacheOptions { KeyPrefix = "test" });
        ICacheService service = new MemoryCacheService(memoryCache, options);

        var user = new TestUser(Guid.NewGuid(), "Ali");

        // Act
        await service.SetAsync("user:1", user, TimeSpan.FromMinutes(1));
        var retrieved = await service.GetAsync<TestUser>("user:1");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(user.Id, retrieved.Id);
        Assert.Equal("Ali", retrieved.Name);
    }

    [Fact]
    public async Task MemoryCacheService_GetOrCreate_ShouldCallFactoryOnce()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new CacheOptions { KeyPrefix = "test" });
        ICacheService service = new MemoryCacheService(memoryCache, options);

        int callCount = 0;
        Task<string> Factory(CancellationToken _)
        {
            callCount++;
            return Task.FromResult("calculated-data");
        }

        // Act
        var first = await service.GetOrCreateAsync("data:1", Factory);
        var second = await service.GetOrCreateAsync("data:1", Factory);

        // Assert
        Assert.Equal("calculated-data", first);
        Assert.Equal("calculated-data", second);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task MemoryCacheService_RemoveByPrefix_ShouldEvictMatchingKeys()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new CacheOptions { KeyPrefix = "test" });
        ICacheService service = new MemoryCacheService(memoryCache, options);

        await service.SetAsync("catalog:item:1", "item1");
        await service.SetAsync("catalog:item:2", "item2");
        await service.SetAsync("users:1", "user1");

        // Act
        await service.RemoveByPrefixAsync("catalog:");

        // Assert
        var item1 = await service.GetAsync<string>("catalog:item:1");
        var item2 = await service.GetAsync<string>("catalog:item:2");
        var user1 = await service.GetAsync<string>("users:1");

        Assert.Null(item1);
        Assert.Null(item2);
        Assert.Equal("user1", user1);
    }

    [Fact]
    public async Task DistributedCacheService_SetAndGet_ShouldSerializeAndDeserializeJson()
    {
        // Arrange (using in-memory distributed cache)
        var distributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var options = Options.Create(new CacheOptions { KeyPrefix = "dist" });
        ICacheService service = new DistributedCacheService(distributedCache, options);

        var user = new TestUser(Guid.NewGuid(), "Veli");

        // Act
        await service.SetAsync("user:2", user, TimeSpan.FromMinutes(2));
        var retrieved = await service.GetAsync<TestUser>("user:2");
        var exists = await service.ExistsAsync("user:2");

        // Assert
        Assert.True(exists);
        Assert.NotNull(retrieved);
        Assert.Equal(user.Id, retrieved.Id);
        Assert.Equal("Veli", retrieved.Name);
    }
}
