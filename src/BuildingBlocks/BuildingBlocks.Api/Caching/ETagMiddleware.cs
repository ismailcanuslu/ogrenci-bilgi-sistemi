using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Caching;

public sealed class ETagMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (!HttpMethods.IsGet(context.Request.Method) &&
            !HttpMethods.IsHead(context.Request.Method))
        {
            return;
        }

        if (context.Response.StatusCode != StatusCodes.Status200OK)
        {
            return;
        }

        if (!context.Response.Headers.TryGetValue(
                "ETag",
                out var responseETag))
        {
            return;
        }

        if (!context.Request.Headers.TryGetValue(
                "If-None-Match",
                out var requestETag))
        {
            return;
        }

        if (!string.Equals(
                responseETag.ToString(),
                requestETag.ToString(),
                StringComparison.Ordinal))
        {
            return;
        }

        context.Response.StatusCode = StatusCodes.Status304NotModified;
        context.Response.ContentLength = 0;
    }
}