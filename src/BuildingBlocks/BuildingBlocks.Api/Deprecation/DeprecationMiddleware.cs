using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Deprecation;

public sealed class DeprecationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        var metadata = context
            .GetEndpoint()?
            .Metadata
            .GetMetadata<DeprecationMetadata>();

        if (metadata is null)
        {
            return;
        }

        context.Response.Headers["Deprecation"] = "true";

        if (metadata.Sunset is not null)
        {
            context.Response.Headers["Sunset"] =
                metadata.Sunset.Value.ToUniversalTime()
                    .ToString("R");
        }
    }
}