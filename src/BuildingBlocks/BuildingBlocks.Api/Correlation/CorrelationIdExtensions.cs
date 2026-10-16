using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Api.Correlation;

public static class CorrelationIdExtensions
{
    public static IApplicationBuilder UseBuildingBlocksCorrelationId(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}