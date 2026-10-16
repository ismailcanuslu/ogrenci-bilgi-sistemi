using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Entities;

namespace Security.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(
        EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.LastLoginAtUtc);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.FailedLoginAttemptCount)
            .IsRequired();

        builder.Property(x => x.LockedUntilUtc);

        builder.OwnsOne(
            x => x.Email,
            email =>
            {
                email.Property(x => x.Value)
                    .HasColumnName("email")
                    .HasMaxLength(320)
                    .IsRequired();

                email.HasIndex(x => x.Value)
                    .IsUnique();
            });

        builder.HasMany(x => x.PasskeyCredentials)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RecoveryCodes)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PasswordCredential)
            .WithOne()
            .HasForeignKey<PasswordCredential>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TotpCredential)
            .WithOne()
            .HasForeignKey<TotpCredential>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}