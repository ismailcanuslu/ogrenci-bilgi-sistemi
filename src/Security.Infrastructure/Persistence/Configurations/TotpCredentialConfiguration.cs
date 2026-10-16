using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Entities;

namespace Security.Infrastructure.Persistence.Configurations;

public sealed class TotpCredentialConfiguration
    : IEntityTypeConfiguration<TotpCredential>
{
    public void Configure(
        EntityTypeBuilder<TotpCredential> builder)
    {
        builder.ToTable("totp_credentials");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.ProtectedSecret)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.EnabledAtUtc);

        builder.Property(x => x.DisabledAtUtc);

        builder.Property(x => x.LastUsedAtUtc);

        builder.HasIndex(x => x.UserId)
            .IsUnique();
    }
}