using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Webhooks.Domain;

namespace Webhooks.Infrastructure.Persistence.Configurations;

public sealed class WebhookDeliveryLogConfiguration : IEntityTypeConfiguration<WebhookDeliveryLog>
{
    public void Configure(EntityTypeBuilder<WebhookDeliveryLog> builder)
    {
        builder.ToTable("WebhookDeliveryLogs", "webhooks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => WebhookDeliveryId.From(value))
            .ValueGeneratedNever();

        builder.Property(x => x.SubscriptionId)
            .HasConversion(id => id.Value, value => WebhookSubscriptionId.From(value))
            .IsRequired();

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Payload)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.StatusCode);

        builder.Property(x => x.IsSuccess)
            .IsRequired();

        builder.Property(x => x.AttemptCount)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(x => x.DispatchedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.SubscriptionId);
        builder.HasIndex(x => x.DispatchedAtUtc);
    }
}
