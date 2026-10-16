using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Authorization;
using Security.Domain.Entities;

namespace Security.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(Role.NameMaxLength);
        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.IsSystem).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasMany(x => x.Permissions)
            .WithOne()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Permissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(x => new { x.RoleId, x.Permission });

        builder.Property(x => x.Permission)
            .IsRequired()
            .HasMaxLength(RolePermission.PermissionMaxLength);
    }
}

public sealed class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.ToTable("user_role_assignments");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.RoleId).IsRequired();
        builder.Property(x => x.ScopeType).IsRequired().HasConversion<int>();
        builder.Property(x => x.ScopeId);
        builder.Property(x => x.AssignedAtUtc).IsRequired();
        builder.Property(x => x.RevocationReason).HasMaxLength(500);

        builder.Ignore(x => x.IsActive);

        // Geçmiş atamalar korunur: rol/kullanıcı silinirken atama kaydı sessizce silinmez.
        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.RevokedAtUtc });
    }
}
