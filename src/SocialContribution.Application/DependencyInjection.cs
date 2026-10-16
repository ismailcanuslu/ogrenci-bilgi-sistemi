using Microsoft.Extensions.DependencyInjection;

namespace SocialContribution.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSocialContributionApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}
