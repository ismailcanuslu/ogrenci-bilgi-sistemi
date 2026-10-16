using Microsoft.Extensions.DependencyInjection;

namespace DocumentManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddDocumentManagementApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}
