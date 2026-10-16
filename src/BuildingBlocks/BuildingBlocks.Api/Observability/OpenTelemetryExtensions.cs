using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Api.Observability;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddBuildingBlocksOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceName =
            configuration["OpenTelemetry:ServiceName"]
            ?? "obs-api";

        var otlpEndpoint = configuration["OpenTelemetry:OtlpEndpoint"]
            ?? configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(serviceName);
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(UniversalTelemetry.PlatformActivitySourceName)
                    .AddSource("OBS.*")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (!string.IsNullOrWhiteSpace(otlpEndpoint) && Uri.TryCreate(otlpEndpoint, UriKind.Absolute, out var endpointUri))
                {
                    tracing.AddOtlpExporter(opt => opt.Endpoint = endpointUri);
                }
                else
                {
                    tracing.AddOtlpExporter();
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(UniversalTelemetry.PlatformActivitySourceName)
                    .AddMeter("OBS.*")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (!string.IsNullOrWhiteSpace(otlpEndpoint) && Uri.TryCreate(otlpEndpoint, UriKind.Absolute, out var endpointUri))
                {
                    metrics.AddOtlpExporter(opt => opt.Endpoint = endpointUri);
                }
                else
                {
                    metrics.AddOtlpExporter();
                }
            });

        return services;
    }
}