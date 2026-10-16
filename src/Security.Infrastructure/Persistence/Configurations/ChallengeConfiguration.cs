using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Challenges;

namespace Security.Infrastructure.Persistence.Configurations;

public sealed class ChallengeConfiguration
    : IEntityTypeConfiguration<Challenge>
{
    public void Configure(
        EntityTypeBuilder<Challenge> builder)
    {
        builder.ToTable("challenges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CodeHash)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.UsedAtUtc);

        builder.Property(x => x.AttemptCount)
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasDiscriminator<string>("challenge_type")
            .HasValue<EmailVerificationChallenge>("email_verification")
            .HasValue<EmailLoginChallenge>("email_login")
            .HasValue<PasswordResetChallenge>("password_reset")
            .HasValue<EmailMfaChallenge>("email_mfa")
            .HasValue<SmsMfaChallenge>("sms_mfa");
    }
}