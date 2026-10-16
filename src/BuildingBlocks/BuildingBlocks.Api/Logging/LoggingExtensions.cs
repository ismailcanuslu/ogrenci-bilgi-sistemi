using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Api.Logging;

public static class LoggingExtensions
{
    public static IApplicationBuilder UseBuildingBlocksRequestLogging(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}