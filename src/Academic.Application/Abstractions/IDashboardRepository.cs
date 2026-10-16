using Academic.Domain.Dashboard;

namespace Academic.Application.Abstractions;

public interface IDashboardRepository
{
    Task<IReadOnlyCollection<UserDashboardWidget>> GetUserWidgetsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDashboardWidget?> GetWidgetByIdAsync(Guid widgetId, CancellationToken cancellationToken = default);
    Task AddWidgetAsync(UserDashboardWidget widget, CancellationToken cancellationToken = default);
    Task RemoveWidgetAsync(UserDashboardWidget widget, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserPersonalShortcut>> GetUserShortcutsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserPersonalShortcut?> GetShortcutByIdAsync(Guid shortcutId, CancellationToken cancellationToken = default);
    Task AddShortcutAsync(UserPersonalShortcut shortcut, CancellationToken cancellationToken = default);
    Task RemoveShortcutAsync(UserPersonalShortcut shortcut, CancellationToken cancellationToken = default);
}
