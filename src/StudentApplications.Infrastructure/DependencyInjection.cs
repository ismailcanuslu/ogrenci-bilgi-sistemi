using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentApplications.Application.Abstractions;
using StudentApplications.Infrastructure.Persistence;
using StudentApplications.Infrastructure.Persistence.Repositories;

namespace StudentApplications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStudentApplicationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=UniversalUniversity;Trusted_Connection=True;TrustServerCertificate=True;";

        services.AddDbContext<StudentApplicationsDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsHistoryTable("__EFMigrationsHistory", StudentApplicationsDbContext.Schema);
            });
        });

        services.AddScoped<IStudentApplicationsUnitOfWork>(sp => sp.GetRequiredService<StudentApplicationsDbContext>());
        services.AddScoped<IStudentApplicationRepository, StudentApplicationRepository>();

        return services;
    }
}
