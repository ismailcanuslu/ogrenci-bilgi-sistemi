using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Api.Security;

public static class SecurityExtensions
{
    public static IApplicationBuilder UseBuildingBlocksSecurityHeaders(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}