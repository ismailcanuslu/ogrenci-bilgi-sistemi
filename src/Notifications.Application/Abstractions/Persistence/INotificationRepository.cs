using Notifications.Domain.Notifications;

namespace Notifications.Application.Abstractions.Persistence;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(
        NotificationId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Notification notification,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<Notification> notifications,
        CancellationToken cancellationToken = default);

    void Update(Notification notification);

    Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? recipient = null,
        NotificationStatus? status = null,
        CancellationToken cancellationToken = default);
}
