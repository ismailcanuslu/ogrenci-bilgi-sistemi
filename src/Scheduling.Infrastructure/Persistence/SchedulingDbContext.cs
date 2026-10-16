using BuildingBlocks.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Scheduling.Domain;

namespace Scheduling.Infrastructure.Persistence;

public sealed class SchedulingDbContext : DbContext, IUnitOfWork
{
    public SchedulingDbContext(DbContextOptions<SchedulingDbContext> options)
        : base(options)
    {
    }

    public DbSet<JobDefinition> JobDefinitions => Set<JobDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
