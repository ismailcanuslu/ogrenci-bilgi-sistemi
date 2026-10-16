using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.Versioning;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddBuildingBlocksApiVersioning(
        this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        });

        return services;
    }

    public static RouteGroupBuilder MapVersionedGroup(
        this IEndpointRouteBuilder endpoints,
        string prefix,
        int majorVersion = 1,
        int minorVersion = 0)
    {
        var version = new ApiVersion(majorVersion, minorVersion);
        var versionSet = endpoints.NewApiVersionSet(prefix)
            .HasApiVersion(version)
            .ReportApiVersions()
            .Build();

        return endpoints.MapGroup($"/api/v{{version:apiVersion}}/{prefix.TrimStart('/')}")
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(version);
    }
}