using Microsoft.EntityFrameworkCore;
using StudentApplications.Application.Abstractions;
using StudentApplications.Domain.Applications;

namespace StudentApplications.Infrastructure.Persistence;

public sealed class StudentApplicationsDbContext : DbContext, IStudentApplicationsUnitOfWork
{
    public const string Schema = "student_applications";

    public StudentApplicationsDbContext(DbContextOptions<StudentApplicationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<StudentApplication> StudentApplications => Set<StudentApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudentApplicationsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
