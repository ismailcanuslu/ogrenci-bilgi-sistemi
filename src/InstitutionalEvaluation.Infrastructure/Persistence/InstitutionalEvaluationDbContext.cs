using Microsoft.EntityFrameworkCore;
using InstitutionalEvaluation.Domain;

namespace InstitutionalEvaluation.Infrastructure.Persistence;

public class InstitutionalEvaluationDbContext : DbContext
{
    public InstitutionalEvaluationDbContext(DbContextOptions<InstitutionalEvaluationDbContext> options)
        : base(options)
    {
    }

    public DbSet<EvaluationMetricSnapshot> EvaluationMetricSnapshots => Set<EvaluationMetricSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EvaluationMetricSnapshot>(b =>
        {
            b.ToTable("EvaluationMetricSnapshots");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.Category, x.MetricKey });

            b.Property(x => x.Category).IsRequired().HasMaxLength(100);
            b.Property(x => x.MetricKey).IsRequired().HasMaxLength(100);
            b.Property(x => x.Title).IsRequired().HasMaxLength(250);
            b.Property(x => x.Unit).HasMaxLength(50);
            b.Property(x => x.Value).HasPrecision(18, 2);
            b.Property(x => x.BreakdownJson).HasMaxLength(8000);
        });
    }
}
