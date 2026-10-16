using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Entities;

namespace Security.Infrastructure.Persistence.Configurations;

public sealed class PasskeyCredentialConfiguration
    : IEntityTypeConfiguration<PasskeyCredential>
{
    public void Configure(
        EntityTypeBuilder<PasskeyCredential> builder)
    {
        builder.ToTable("passkey_credentials");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CredentialId)
            .IsRequired();

        builder.HasIndex(x => x.CredentialId)
            .IsUnique();

        builder.Property(x => x.PublicKey)
            .IsRequired();

        builder.Property(x => x.SignCount)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.AuthenticatorType)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUsedAt);

        builder.Property(x => x.IsRevoked)
            .IsRequired();

        builder.Property(x => x.RevokedAt);

        builder.Property(x => x.IsMfaEnabled)
            .IsRequired();
    }
}