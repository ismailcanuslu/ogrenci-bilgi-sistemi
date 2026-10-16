using Microsoft.Extensions.DependencyInjection;
using Translations.Application.Abstractions;
using Translations.Infrastructure.Services;

namespace Translations.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTranslationsInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITranslationService, TranslationService>();

        return services;
    }
}
