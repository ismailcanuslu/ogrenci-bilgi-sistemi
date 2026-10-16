using Academic.Domain.Dashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class UserDashboardWidgetConfiguration : IEntityTypeConfiguration<UserDashboardWidget>
{
    public void Configure(EntityTypeBuilder<UserDashboardWidget> builder)
    {
        builder.ToTable("UserDashboardWidgets");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.CustomTitle).HasMaxLength(UserDashboardWidget.CustomTitleMaxLength);
        builder.Property(x => x.ConfigurationJson).HasMaxLength(UserDashboardWidget.ConfigMaxLength);
        builder.HasIndex(x => new { x.UserId, x.Type });
    }
}

public sealed class UserPersonalShortcutConfiguration : IEntityTypeConfiguration<UserPersonalShortcut>
{
    public void Configure(EntityTypeBuilder<UserPersonalShortcut> builder)
    {
        builder.ToTable("UserPersonalShortcuts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(UserPersonalShortcut.TitleMaxLength);
        builder.Property(x => x.TargetUrl).IsRequired().HasMaxLength(UserPersonalShortcut.UrlMaxLength);
        builder.Property(x => x.Icon).HasMaxLength(UserPersonalShortcut.IconMaxLength);
        builder.Property(x => x.Color).HasMaxLength(UserPersonalShortcut.ColorMaxLength);
        builder.HasIndex(x => x.UserId);
    }
}
