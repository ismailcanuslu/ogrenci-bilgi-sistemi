using Academic.Application.Abstractions;
using Academic.Infrastructure.Persistence;
using Academic.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Academic.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAcademicInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<AcademicDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IAcademicUnitOfWork>(
            provider => provider.GetRequiredService<AcademicDbContext>());

        services.AddScoped<IFacultyRepository, FacultyRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
        services.AddScoped<IProgramRepository, ProgramRepository>();
        services.AddScoped<ICurriculumRepository, CurriculumRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseOfferingRepository, CourseOfferingRepository>();
        services.AddScoped<IInstructorAffiliationRepository, InstructorAffiliationRepository>();
        services.AddScoped<IStudentPortalRepository, StudentPortalRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IEducationCatalogRepository, EducationCatalogRepository>();

        services.AddHostedService<AcademicDataSeeder>();

        return services;
    }
}
