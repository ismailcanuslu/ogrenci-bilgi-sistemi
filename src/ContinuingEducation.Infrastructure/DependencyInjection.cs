using ContinuingEducation.Application.Abstractions;
using ContinuingEducation.Infrastructure.Persistence;
using ContinuingEducation.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContinuingEducation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddContinuingEducationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=UniversalUniversity;Trusted_Connection=True;TrustServerCertificate=True;";

        services.AddDbContext<ContinuingEducationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsHistoryTable("__EFMigrationsHistory", ContinuingEducationDbContext.Schema);
            });
        });

        services.AddScoped<IContinuingEducationUnitOfWork>(sp => sp.GetRequiredService<ContinuingEducationDbContext>());
        services.AddScoped<IEducationProgramRepository, EducationProgramRepository>();
        services.AddScoped<IProgramApplicationRepository, ProgramApplicationRepository>();

        return services;
    }
}
