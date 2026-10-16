using BuildingBlocks.Api.Caching;
using BuildingBlocks.Api.Correlation;
using BuildingBlocks.Api.Deprecation;
using BuildingBlocks.Api.Idempotency;
using BuildingBlocks.Api.Logging;
using BuildingBlocks.Api.RateLimiting;
using BuildingBlocks.Api.Security;
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Api;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseBuildingBlocksApi(
        this WebApplication app)
    {
        app.UseExceptionHandler();

        app.UseBuildingBlocksCorrelationId();

        app.UseBuildingBlocksRequestLogging();

        app.UseBuildingBlocksSecurityHeaders();

        app.UseCors(CorsExtensions.PolicyName);

        app.UseBuildingBlocksRateLimiting();

        app.UseBuildingBlocksIdempotency();

        app.UseBuildingBlocksETag();

        app.UseBuildingBlocksDeprecation();

        return app;
    }
}