using BuildingBlocks.Application.Abstractions.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.Caching;

public static class CachingExtensions
{
    public static IServiceCollection AddBuildingBlocksCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMemoryCache();

        var cacheSection = configuration.GetSection(CacheOptions.SectionName);
        services.Configure<CacheOptions>(cacheSection);

        var options = cacheSection.Get<CacheOptions>() ?? new CacheOptions();
        var redisConn = configuration.GetConnectionString("Redis")
            ?? options.RedisConnectionString
            ?? configuration["REDIS_CONNECTION_STRING"];

        if (string.Equals(options.Provider, "Redis", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(redisConn))
        {
            services.AddStackExchangeRedisCache(redisOpt =>
            {
                redisOpt.Configuration = redisConn;
                redisOpt.InstanceName = $"{options.KeyPrefix}_";
            });

            services.AddSingleton<ICacheService, DistributedCacheService>();
        }
        else
        {
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
        }

        return services;
    }

    public static IApplicationBuilder UseBuildingBlocksETag(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<ETagMiddleware>();
    }
}