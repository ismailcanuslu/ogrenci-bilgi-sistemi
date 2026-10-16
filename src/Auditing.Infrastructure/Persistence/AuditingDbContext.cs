using Auditing.Domain;
using BuildingBlocks.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auditing.Infrastructure.Persistence;

public sealed class AuditingDbContext : DbContext, IUnitOfWork
{
    public AuditingDbContext(DbContextOptions<AuditingDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
