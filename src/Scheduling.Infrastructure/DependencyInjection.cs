using BuildingBlocks.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduling.Application.Abstractions;
using Scheduling.Infrastructure.Background;
using Scheduling.Infrastructure.Persistence;
using Scheduling.Infrastructure.Services;

namespace Scheduling.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSchedulingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<SchedulingDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<SchedulingDbContext>());

        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<ISchedulerService, SchedulerService>();

        services.AddHostedService<JobSchedulerBackgroundService>();

        return services;
    }
}
