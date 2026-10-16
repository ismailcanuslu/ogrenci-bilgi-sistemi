using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace SupportDesk.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportDeskApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
