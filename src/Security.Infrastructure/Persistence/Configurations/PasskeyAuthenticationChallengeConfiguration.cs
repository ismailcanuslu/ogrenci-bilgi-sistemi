using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Challenges;

namespace Security.Infrastructure.Persistence.Configurations;

public sealed class PasskeyAuthenticationChallengeConfiguration
    : IEntityTypeConfiguration<PasskeyAuthenticationChallenge>
{
    public void Configure(
        EntityTypeBuilder<PasskeyAuthenticationChallenge> builder)
    {
        builder.ToTable("passkey_authentication_challenges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.OptionsJson)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.UsedAtUtc);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.ExpiresAtUtc);
    }
}