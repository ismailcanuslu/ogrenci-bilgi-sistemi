using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialContribution.Application.Abstractions;
using SocialContribution.Infrastructure.Persistence;

namespace SocialContribution.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSocialContributionInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SocialContributionDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(SocialContributionDbContext).Assembly.FullName)));

        services.AddScoped<ISocialContributionRepository, SocialContributionRepository>();

        return services;
    }
}
