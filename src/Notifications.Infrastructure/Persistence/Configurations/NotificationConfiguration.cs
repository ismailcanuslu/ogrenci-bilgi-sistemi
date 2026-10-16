using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notifications.Domain.Notifications;
using Notifications.Domain.ValueObjects;

namespace Notifications.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new NotificationId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Recipient)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasConversion(
                s => s != null ? s.Value : null,
                v => v != null ? Subject.Create(v).Value : null)
            .HasMaxLength(255);

        builder.OwnsOne(x => x.Body, body =>
        {
            body.Property(b => b.Value)
                .HasColumnName("body")
                .IsRequired();

            body.Property(b => b.IsHtml)
                .HasColumnName("is_html")
                .IsRequired();
        });

        builder.Property(x => x.TemplateCode)
            .HasMaxLength(100);

        builder.Property(x => x.TemplateDataJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000);

        builder.Property(x => x.RetryCount)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.SentAt);
    }
}
