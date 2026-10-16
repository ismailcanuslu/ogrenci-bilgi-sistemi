using Academic.Application.Abstractions;
using Academic.Domain.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly AcademicDbContext _dbContext;

    public DashboardRepository(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<UserDashboardWidget>> GetUserWidgetsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserDashboardWidgets
            .Where(w => w.UserId == userId)
            .OrderBy(w => w.Order)
            .ToListAsync(cancellationToken);
    }

    public Task<UserDashboardWidget?> GetWidgetByIdAsync(Guid widgetId, CancellationToken cancellationToken = default)
    {
        return _dbContext.UserDashboardWidgets.FirstOrDefaultAsync(w => w.Id == widgetId, cancellationToken);
    }

    public async Task AddWidgetAsync(UserDashboardWidget widget, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserDashboardWidgets.AddAsync(widget, cancellationToken);
    }

    public Task RemoveWidgetAsync(UserDashboardWidget widget, CancellationToken cancellationToken = default)
    {
        _dbContext.UserDashboardWidgets.Remove(widget);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyCollection<UserPersonalShortcut>> GetUserShortcutsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserPersonalShortcuts
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.Order)
            .ToListAsync(cancellationToken);
    }

    public Task<UserPersonalShortcut?> GetShortcutByIdAsync(Guid shortcutId, CancellationToken cancellationToken = default)
    {
        return _dbContext.UserPersonalShortcuts.FirstOrDefaultAsync(s => s.Id == shortcutId, cancellationToken);
    }

    public async Task AddShortcutAsync(UserPersonalShortcut shortcut, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserPersonalShortcuts.AddAsync(shortcut, cancellationToken);
    }

    public Task RemoveShortcutAsync(UserPersonalShortcut shortcut, CancellationToken cancellationToken = default)
    {
        _dbContext.UserPersonalShortcuts.Remove(shortcut);
        return Task.CompletedTask;
    }
}
