using Microsoft.Extensions.DependencyInjection;

namespace Webhooks.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWebhooksApplication(this IServiceCollection services)
    {
        return services;
    }
}
