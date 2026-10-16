using Microsoft.Extensions.DependencyInjection;

namespace ContinuingEducation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddContinuingEducationApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}
