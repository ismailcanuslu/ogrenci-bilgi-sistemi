using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.Health;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddBuildingBlocksHealthChecks(
        this IServiceCollection services)
    {
        services.AddHealthChecks();

        return services;
    }

    public static IEndpointRouteBuilder MapBuildingBlocksHealthChecks(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks(
            "/health",
            new HealthCheckOptions
            {
                AllowCachingResponses = false
            });

        return endpoints;
    }
}