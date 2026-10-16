using Messaging.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messaging.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages", "messaging");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new OutboxMessageId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.OccurredOnUtc)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.PayloadJson)
            .IsRequired();

        builder.Property(x => x.ProcessedOnUtc)
            .IsRequired(false);

        builder.Property(x => x.Error)
            .IsRequired(false);

        builder.Property(x => x.RetryCount)
            .IsRequired();

        builder.Property(x => x.TraceId)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.SpanId)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.HasIndex(x => x.ProcessedOnUtc);
        builder.HasIndex(x => x.OccurredOnUtc);
        builder.HasIndex(x => x.TraceId);
    }
}
