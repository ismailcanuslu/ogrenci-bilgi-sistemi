using ContinuingEducation.Application.Abstractions;
using ContinuingEducation.Domain.Programs;
using Microsoft.EntityFrameworkCore;

namespace ContinuingEducation.Infrastructure.Persistence;

public sealed class ContinuingEducationDbContext : DbContext, IContinuingEducationUnitOfWork
{
    public const string Schema = "continuing_education";

    public ContinuingEducationDbContext(DbContextOptions<ContinuingEducationDbContext> options)
        : base(options)
    {
    }

    public DbSet<EducationProgram> EducationPrograms => Set<EducationProgram>();

    public DbSet<ProgramApplication> ProgramApplications => Set<ProgramApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContinuingEducationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
