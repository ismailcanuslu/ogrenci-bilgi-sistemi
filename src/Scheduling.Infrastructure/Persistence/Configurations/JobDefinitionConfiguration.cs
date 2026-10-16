using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scheduling.Domain;

namespace Scheduling.Infrastructure.Persistence.Configurations;

public sealed class JobDefinitionConfiguration : IEntityTypeConfiguration<JobDefinition>
{
    public void Configure(EntityTypeBuilder<JobDefinition> builder)
    {
        builder.ToTable("JobDefinitions", "scheduling");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, val => new JobId(val))
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.CronExpression)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.PayloadJson)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.NextExecutionTimeUtc)
            .IsRequired(false);

        builder.Property(x => x.LastExecutedTimeUtc)
            .IsRequired(false);

        builder.Property(x => x.RetryCount)
            .IsRequired();

        builder.Property(x => x.MaxRetries)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.NextExecutionTimeUtc);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Name);
    }
}
