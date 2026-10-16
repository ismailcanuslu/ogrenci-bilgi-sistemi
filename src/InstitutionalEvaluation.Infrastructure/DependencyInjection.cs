using Academic.Infrastructure.Persistence;
using InstitutionalEvaluation.Application.Abstractions;
using InstitutionalEvaluation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InstitutionalEvaluation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInstitutionalEvaluationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InstitutionalEvaluationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(InstitutionalEvaluationDbContext).Assembly.FullName)));

        services.AddScoped<IInstitutionalEvaluationRepository, InstitutionalEvaluationRepository>();

        return services;
    }
}
