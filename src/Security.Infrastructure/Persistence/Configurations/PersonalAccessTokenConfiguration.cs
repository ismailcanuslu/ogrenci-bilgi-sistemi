using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Authorization;

namespace Security.Infrastructure.Persistence.Configurations;

public sealed class PersonalAccessTokenConfiguration : IEntityTypeConfiguration<PersonalAccessToken>
{
    public void Configure(EntityTypeBuilder<PersonalAccessToken> builder)
    {
        builder.ToTable("personal_access_tokens", "security");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(PersonalAccessToken.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasMaxLength(PersonalAccessToken.TokenHashMaxLength)
            .IsRequired();

        builder.Property(x => x.MaskedToken)
            .HasMaxLength(PersonalAccessToken.MaskedTokenMaxLength)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId).IsRequired();
        builder.Property(x => x.ScopeType).IsRequired();
        builder.Property(x => x.ScopeId);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.ExpiresAtUtc);
        builder.Property(x => x.RevokedAtUtc);
        builder.Property(x => x.RevokedByUserId);
        builder.Property(x => x.LastUsedAtUtc);

        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => x.CreatedByUserId);
        builder.HasIndex(x => x.RevokedAtUtc);

        builder.HasMany(x => x.Permissions)
            .WithOne()
            .HasForeignKey(x => x.PersonalAccessTokenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Permissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class PersonalAccessTokenPermissionConfiguration : IEntityTypeConfiguration<PersonalAccessTokenPermission>
{
    public void Configure(EntityTypeBuilder<PersonalAccessTokenPermission> builder)
    {
        builder.ToTable("personal_access_token_permissions", "security");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PersonalAccessTokenId).IsRequired();

        builder.Property(x => x.Permission)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => new { x.PersonalAccessTokenId, x.Permission }).IsUnique();
        builder.HasIndex(x => x.Permission);
    }
}
