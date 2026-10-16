using Microsoft.Extensions.DependencyInjection;

namespace StudentApplications.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddStudentApplicationsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}
