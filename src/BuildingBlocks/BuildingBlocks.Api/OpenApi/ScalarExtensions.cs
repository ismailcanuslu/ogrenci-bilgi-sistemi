using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace BuildingBlocks.Api.OpenApi;

public static class ScalarExtensions
{
    /// <summary>
    /// OpenAPI servisini DI konteynerine kaydeder.
    /// </summary>
    public static IServiceCollection AddBuildingBlocksOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    /// <summary>
    /// /openapi/v1.json şemasını ve modern etkileşimli Scalar API arayüzünü (/scalar/v1) haritalandırır.
    /// </summary>
    public static WebApplication MapBuildingBlocksOpenApiAndScalar(this WebApplication app, string title = "OBS Platform API")
    {
        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options.WithTitle(title)
                   .WithTheme(ScalarTheme.DeepSpace)
                   .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        return app;
    }
}
