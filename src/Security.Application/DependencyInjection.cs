using BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Security.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSecurityApplication(
        this IServiceCollection services)
    {
        return services.AddModuleApplication(typeof(DependencyInjection).Assembly);
    }
}
