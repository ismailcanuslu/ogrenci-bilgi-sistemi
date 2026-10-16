using Microsoft.EntityFrameworkCore;
using Webhooks.Application.Abstractions;
using Webhooks.Domain;

namespace Webhooks.Infrastructure.Persistence;

public sealed class WebhookRepository : IWebhookRepository
{
    private readonly WebhooksDbContext _dbContext;

    public WebhookRepository(WebhooksDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<WebhookSubscription?> GetByIdAsync(WebhookSubscriptionId id, CancellationToken ct = default)
    {
        return await _dbContext.Subscriptions
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<WebhookSubscription>> GetActiveSubscriptionsForEventAsync(string eventType, CancellationToken ct = default)
    {
        var activeSubscriptions = await _dbContext.Subscriptions
            .Where(x => x.IsActive)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return activeSubscriptions
            .Where(x => x.SubscribesTo(eventType))
            .ToList();
    }

    public async Task<IReadOnlyList<WebhookSubscription>> GetAllSubscriptionsAsync(CancellationToken ct = default)
    {
        return await _dbContext.Subscriptions
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(WebhookSubscription subscription, CancellationToken ct = default)
    {
        await _dbContext.Subscriptions.AddAsync(subscription, ct).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task UpdateAsync(WebhookSubscription subscription, CancellationToken ct = default)
    {
        _dbContext.Subscriptions.Update(subscription);
        await _dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task DeleteAsync(WebhookSubscription subscription, CancellationToken ct = default)
    {
        _dbContext.Subscriptions.Remove(subscription);
        await _dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task AddDeliveryLogAsync(WebhookDeliveryLog log, CancellationToken ct = default)
    {
        await _dbContext.DeliveryLogs.AddAsync(log, ct).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<WebhookDeliveryLog>> GetDeliveryLogsAsync(WebhookSubscriptionId? subscriptionId, int limit = 50, CancellationToken ct = default)
    {
        var query = _dbContext.DeliveryLogs.AsQueryable();

        if (subscriptionId.HasValue)
        {
            query = query.Where(x => x.SubscriptionId == subscriptionId.Value);
        }

        return await query
            .OrderByDescending(x => x.DispatchedAtUtc)
            .Take(limit)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
}
