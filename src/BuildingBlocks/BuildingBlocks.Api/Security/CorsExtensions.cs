using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.Security;

public static class CorsExtensions
{
    public const string PolicyName = "BuildingBlocksCors";

    public static IServiceCollection AddBuildingBlocksCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var origins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(
                PolicyName,
                policy =>
                {
                    if (origins.Length == 0)
                    {
                        return;
                    }

                    policy
                        .WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        return services;
    }
}