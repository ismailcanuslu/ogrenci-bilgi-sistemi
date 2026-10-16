using BuildingBlocks.Api.Caching;
using BuildingBlocks.Api.Features;
using BuildingBlocks.Api.Health;
using BuildingBlocks.Api.Idempotency;
using BuildingBlocks.Api.Observability;
using BuildingBlocks.Api.RateLimiting;
using BuildingBlocks.Api.Security;
using BuildingBlocks.Api.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocksApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddProblemDetails();

        services.AddExceptionHandler<Exceptions.GlobalExceptionHandler>();

        services.AddBuildingBlocksApiVersioning();

        services.AddBuildingBlocksHealthChecks();

        services.AddBuildingBlocksRateLimiting();

        services.AddBuildingBlocksCors(configuration);

        services.AddBuildingBlocksIdempotency();

        services.AddBuildingBlocksCaching(configuration);

        services.AddBuildingBlocksFeatureManagement(configuration);

        services.AddBuildingBlocksOpenTelemetry(configuration);

        services.AddOpenApi();

        return services;
    }
}