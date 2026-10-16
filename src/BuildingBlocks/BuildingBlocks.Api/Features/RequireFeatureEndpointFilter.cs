using BuildingBlocks.Application.Abstractions.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.Features;

/// <summary>
/// Minimal API endpoint'leri için Feature Flag kontrol filtresi.
/// Belirtilen özellik kapalıysa HTTP 404 Not Found (RFC 9457 Problem Details) döner.
/// </summary>
public sealed class RequireFeatureEndpointFilter : IEndpointFilter
{
    private readonly string _featureName;

    public RequireFeatureEndpointFilter(string featureName)
    {
        _featureName = featureName ?? throw new ArgumentNullException(nameof(featureName));
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var featureService = context.HttpContext.RequestServices.GetService<IFeatureService>();

        if (featureService != null)
        {
            var isEnabled = await featureService.IsEnabledAsync(_featureName, context.HttpContext.RequestAborted).ConfigureAwait(false);
            if (!isEnabled)
            {
                return Results.Problem(
                    title: "Feature Disabled",
                    detail: $"The requested feature '{_featureName}' is currently disabled.",
                    statusCode: StatusCodes.Status404NotFound,
                    instance: context.HttpContext.Request.Path);
            }
        }

        return await next(context);
    }
}

public static class FeatureFilterExtensions
{
    /// <summary>
    /// Bu endpoint'in çalışması için belirtilen Feature Flag'in aktif olmasını şart koşar.
    /// </summary>
    public static RouteHandlerBuilder RequireFeature(this RouteHandlerBuilder builder, string featureName)
    {
        return builder.AddEndpointFilter(new RequireFeatureEndpointFilter(featureName));
    }
}
