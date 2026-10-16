using Auditing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auditing.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "auditing");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new AuditLogId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired(false);

        builder.Property(x => x.UserName)
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(x => x.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Module)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Resource)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ResourceId)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.OldValuesJson)
            .IsRequired(false);

        builder.Property(x => x.NewValuesJson)
            .IsRequired(false);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(512)
            .IsRequired(false);

        builder.Property(x => x.TraceId)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.SpanId)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.Property(x => x.IsSuccess)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.HasIndex(x => x.Timestamp);
        builder.HasIndex(x => new { x.Module, x.Action });
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.TraceId);
    }
}
