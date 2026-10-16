using Microsoft.EntityFrameworkCore;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Domain.Notifications;

namespace Notifications.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository(NotificationsDbContext context) : INotificationRepository
{
    public async Task<Notification?> GetByIdAsync(
        NotificationId id,
        CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        Notification notification,
        CancellationToken cancellationToken = default)
    {
        await context.Notifications.AddAsync(notification, cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<Notification> notifications,
        CancellationToken cancellationToken = default)
    {
        await context.Notifications.AddRangeAsync(notifications, cancellationToken);
    }

    public void Update(Notification notification)
    {
        context.Notifications.Update(notification);
    }

    public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? recipient = null,
        NotificationStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Notifications.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(recipient))
        {
            query = query.Where(x => x.Recipient.Contains(recipient));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
