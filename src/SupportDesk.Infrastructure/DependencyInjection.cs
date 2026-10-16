using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Application.Abstractions;
using SupportDesk.Infrastructure.Persistence;
using SupportDesk.Infrastructure.Persistence.Repositories;

namespace SupportDesk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportDeskInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<SupportDeskDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<ISupportDeskUnitOfWork>(
            provider => provider.GetRequiredService<SupportDeskDbContext>());

        services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();

        return services;
    }
}
