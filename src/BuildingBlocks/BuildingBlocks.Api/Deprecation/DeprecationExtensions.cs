using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Deprecation;

public static class DeprecationExtensions
{
    public static TBuilder WithDeprecation<TBuilder>(
        this TBuilder builder,
        DateTimeOffset? sunset = null)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.Add(endpointBuilder =>
        {
            endpointBuilder.Metadata.Add(
                new DeprecationMetadata(sunset));
        });

        return builder;
    }

    public static IApplicationBuilder UseBuildingBlocksDeprecation(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<DeprecationMiddleware>();
    }
}

public sealed record DeprecationMetadata(
    DateTimeOffset? Sunset);