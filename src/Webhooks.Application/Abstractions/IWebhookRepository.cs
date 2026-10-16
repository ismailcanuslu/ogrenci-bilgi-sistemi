using Webhooks.Domain;

namespace Webhooks.Application.Abstractions;

public interface IWebhookRepository
{
    Task<WebhookSubscription?> GetByIdAsync(WebhookSubscriptionId id, CancellationToken ct = default);
    Task<IReadOnlyList<WebhookSubscription>> GetActiveSubscriptionsForEventAsync(string eventType, CancellationToken ct = default);
    Task<IReadOnlyList<WebhookSubscription>> GetAllSubscriptionsAsync(CancellationToken ct = default);
    Task AddAsync(WebhookSubscription subscription, CancellationToken ct = default);
    Task UpdateAsync(WebhookSubscription subscription, CancellationToken ct = default);
    Task DeleteAsync(WebhookSubscription subscription, CancellationToken ct = default);

    Task AddDeliveryLogAsync(WebhookDeliveryLog log, CancellationToken ct = default);
    Task<IReadOnlyList<WebhookDeliveryLog>> GetDeliveryLogsAsync(WebhookSubscriptionId? subscriptionId, int limit = 50, CancellationToken ct = default);
}
