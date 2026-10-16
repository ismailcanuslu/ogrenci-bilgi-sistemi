using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.RateLimiting;

public static class RateLimitingExtensions
{
    public const string FixedPolicy = "fixed";
    public const string PerUserPolicy = "per-user";

    public static IServiceCollection AddBuildingBlocksRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter(
                FixedPolicy,
                limiterOptions =>
                {
                    limiterOptions.PermitLimit = 100;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.QueueLimit = 0;
                });

            options.AddPolicy(
                PerUserPolicy,
                context =>
                {
                    var userId =
                        context.User.Identity?.Name
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        userId,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });
        });

        return services;
    }

    public static IApplicationBuilder UseBuildingBlocksRateLimiting(
        this IApplicationBuilder app)
    {
        return app.UseRateLimiter();
    }
}