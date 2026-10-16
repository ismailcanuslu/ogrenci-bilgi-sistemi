using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMessagingApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
