using DocumentManagement.Application.Abstractions;
using DocumentManagement.Infrastructure.Persistence;
using DocumentManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDocumentManagementInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=UniversalUniversity;Trusted_Connection=True;TrustServerCertificate=True;";

        services.AddDbContext<DocumentManagementDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsHistoryTable("__EFMigrationsHistory", DocumentManagementDbContext.Schema);
            });
        });

        services.AddScoped<IDocumentManagementUnitOfWork>(sp => sp.GetRequiredService<DocumentManagementDbContext>());
        services.AddScoped<IOfficialDocumentRepository, OfficialDocumentRepository>();

        return services;
    }
}
