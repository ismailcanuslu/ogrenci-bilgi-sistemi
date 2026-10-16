using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Translations.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTranslationsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
