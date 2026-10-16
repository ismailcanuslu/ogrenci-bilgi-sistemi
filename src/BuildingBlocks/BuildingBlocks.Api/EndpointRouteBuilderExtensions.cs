using BuildingBlocks.Api.Features;
using BuildingBlocks.Api.Health;
using Microsoft.AspNetCore.Builder;
using Scalar.AspNetCore;

namespace BuildingBlocks.Api;

public static class EndpointRouteBuilderExtensions
{
    public static WebApplication MapBuildingBlocksApi(
        this WebApplication app,
        string apiTitle = "OBS Platform API")
    {
        app.MapBuildingBlocksHealthChecks();
        app.MapBuildingBlocksFeaturesEndpoint();

        // OpenAPI Şeması (/openapi/v1.json)
        app.MapOpenApi();

        // Etkileşimli Scalar API Dokümantasyonu (/scalar/v1)
        app.MapScalarApiReference(options =>
        {
            options.WithTitle(apiTitle)
                   .WithTheme(ScalarTheme.DeepSpace)
                   .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        return app;
    }
}