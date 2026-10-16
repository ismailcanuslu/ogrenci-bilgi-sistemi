using CampusLife.Application.Abstractions;
using CampusLife.Infrastructure.Persistence;
using CampusLife.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampusLife.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCampusLifeInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<CampusLifeDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<ICampusLifeUnitOfWork>(
            provider => provider.GetRequiredService<CampusLifeDbContext>());

        services.AddScoped<IFacilityReservationRepository, FacilityReservationRepository>();
        services.AddScoped<IStudentClubRepository, StudentClubRepository>();

        return services;
    }
}
