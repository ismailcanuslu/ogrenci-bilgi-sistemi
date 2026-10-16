using System.Diagnostics;
using BuildingBlocks.Api.Correlation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Api.Logging;

public sealed class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            var correlationId = context.Items
                .TryGetValue(
                    CorrelationIdMiddleware.HeaderName,
                    out var value)
                ? value?.ToString()
                : null;

            logger.LogInformation(
                "HTTP request completed. " +
                "Method: {Method}, " +
                "Path: {Path}, " +
                "StatusCode: {StatusCode}, " +
                "ElapsedMs: {ElapsedMs}, " +
                "CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds,
                correlationId);
        }
    }
}