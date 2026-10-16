using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Idempotency;

public sealed class IdempotencyMiddleware(
    RequestDelegate next,
    IIdempotencyStore store)
{
    public const string HeaderName = "Idempotency-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!ShouldProcess(context))
        {
            await next(context);
            return;
        }

        var key = context.Request.Headers[HeaderName].ToString();

        var existingEntry = await store.GetAsync(
            key,
            context.RequestAborted);

        if (existingEntry is not null)
        {
            await WriteCachedResponseAsync(
                context,
                existingEntry);

            return;
        }

        var originalResponseBody = context.Response.Body;

        await using var responseBody = new MemoryStream();

        context.Response.Body = responseBody;

        try
        {
            await next(context);

            responseBody.Position = 0;

            var responseBytes = responseBody.ToArray();

            var entry = new IdempotencyEntry(
                key,
                context.Response.StatusCode,
                context.Response.ContentType,
                responseBytes);

            await store.TryCreateAsync(
                entry,
                context.RequestAborted);

            responseBody.Position = 0;

            await responseBody.CopyToAsync(
                originalResponseBody,
                context.RequestAborted);
        }
        finally
        {
            context.Response.Body = originalResponseBody;
        }
    }

    private static bool ShouldProcess(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method) &&
            !HttpMethods.IsPut(context.Request.Method) &&
            !HttpMethods.IsPatch(context.Request.Method))
        {
            return false;
        }

        return context.Request.Headers.ContainsKey(HeaderName);
    }

    private static async Task WriteCachedResponseAsync(
        HttpContext context,
        IdempotencyEntry entry)
    {
        context.Response.StatusCode = entry.StatusCode;

        if (!string.IsNullOrWhiteSpace(entry.ContentType))
        {
            context.Response.ContentType = entry.ContentType;
        }

        await context.Response.Body.WriteAsync(
            entry.ResponseBody,
            context.RequestAborted);
    }
}