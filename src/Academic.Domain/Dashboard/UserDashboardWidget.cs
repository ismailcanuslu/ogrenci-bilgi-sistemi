using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Dashboard;

public sealed class UserDashboardWidget : Entity<Guid>
{
    public const int CustomTitleMaxLength = 100;
    public const int ConfigMaxLength = 2000;

    public Guid UserId { get; private set; }

    public WidgetType Type { get; private set; }

    public string? CustomTitle { get; private set; }

    public int Order { get; private set; }

    public int ColumnSpan { get; private set; } // 1, 2 (yarım veya tam genişlik)

    public string? ConfigurationJson { get; private set; }

    public bool IsVisible { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private UserDashboardWidget()
    {
    }

    public UserDashboardWidget(
        Guid id,
        Guid userId,
        WidgetType type,
        string? customTitle,
        int order,
        int columnSpan,
        string? configurationJson,
        DateTime createdAtUtc)
        : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("UserDashboardWidget.UserRequired");
        }

        if (order < 0)
        {
            throw new DomainException("UserDashboardWidget.InvalidOrder");
        }

        UserId = userId;
        Type = type;
        CustomTitle = customTitle?.Trim();
        Order = order;
        ColumnSpan = columnSpan <= 0 ? 1 : columnSpan;
        ConfigurationJson = configurationJson?.Trim();
        IsVisible = true;
        CreatedAtUtc = createdAtUtc;
    }

    public void UpdateSettings(string? customTitle, int order, int columnSpan, string? configurationJson, bool isVisible)
    {
        CustomTitle = customTitle?.Trim();
        Order = Math.Max(0, order);
        ColumnSpan = columnSpan <= 0 ? 1 : columnSpan;
        ConfigurationJson = configurationJson?.Trim();
        IsVisible = isVisible;
    }

    public void SetOrder(int order)
    {
        Order = Math.Max(0, order);
    }

    public void ToggleVisibility(bool isVisible)
    {
        IsVisible = isVisible;
    }
}
