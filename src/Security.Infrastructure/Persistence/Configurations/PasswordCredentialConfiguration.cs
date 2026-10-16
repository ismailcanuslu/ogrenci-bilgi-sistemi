using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Entities;

namespace Security.Infrastructure.Persistence.Configurations;

public sealed class PasswordCredentialConfiguration
    : IEntityTypeConfiguration<PasswordCredential>
{
    public void Configure(
        EntityTypeBuilder<PasswordCredential> builder)
    {
        builder.ToTable("password_credentials");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.LastUsedAtUtc);

        builder.Property(x => x.PasswordChangedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .IsUnique();
    }
}