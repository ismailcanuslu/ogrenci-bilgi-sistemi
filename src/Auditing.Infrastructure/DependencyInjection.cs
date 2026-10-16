using Auditing.Application.Abstractions;
using Auditing.Infrastructure.Persistence;
using Auditing.Infrastructure.Repositories;
using Auditing.Infrastructure.Services;
using BuildingBlocks.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auditing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<AuditingDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<AuditingDbContext>());

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
