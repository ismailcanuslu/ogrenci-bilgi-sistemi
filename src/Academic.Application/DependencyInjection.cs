using Academic.Application.Abstractions;
using BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Academic.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAcademicApplication(
        this IServiceCollection services)
    {
        services.AddModuleApplication(typeof(DependencyInjection).Assembly);
        services.AddScoped<AcademicAccess>();

        return services;
    }
}
