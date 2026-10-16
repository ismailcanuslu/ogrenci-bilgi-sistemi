using BuildingBlocks.Application.Abstractions.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace BuildingBlocks.Api.Features;

public static class FeatureManagementExtensions
{
    /// <summary>
    /// Microsoft.FeatureManagement kütüphanesini ve BuildingBlocks IFeatureService sarmalayıcısını sisteme kaydeder.
    /// </summary>
    public static IServiceCollection AddBuildingBlocksFeatureManagement(this IServiceCollection services, IConfiguration? configuration = null)
    {
        services.AddFeatureManagement();
        services.AddSingleton<IFeatureService, FeatureService>();
        return services;
    }

    /// <summary>
    /// Aktif/pasif feature flag'leri sorgulamak için standart REST endpoint'lerini ekler.
    /// </summary>
    public static IEndpointRouteBuilder MapBuildingBlocksFeaturesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/features")
            .WithTags("Features");

        group.MapGet("/", async (IFeatureService featureService, CancellationToken ct) =>
        {
            var features = await featureService.GetAllFeaturesAsync(ct);
            return Results.Ok(features);
        })
        .WithName("GetAllFeatures")
        .WithSummary("Sistemdeki tüm Feature Flag durumlarını listeler.");

        group.MapGet("/{name}", async (string name, IFeatureService featureService, CancellationToken ct) =>
        {
            var isEnabled = await featureService.IsEnabledAsync(name, ct);
            return Results.Ok(new { Feature = name, IsEnabled = isEnabled });
        })
        .WithName("GetFeatureStatus")
        .WithSummary("Belirli bir Feature Flag'in aktiflik durumunu sorgular.");

        return endpoints;
    }
}
