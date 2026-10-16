using BuildingBlocks.Domain.Primitives;

namespace Webhooks.Domain;

public sealed class WebhookDeliveryLog : Entity<WebhookDeliveryId>
{
    public WebhookSubscriptionId SubscriptionId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public int? StatusCode { get; private set; }
    public bool IsSuccess { get; private set; }
    public int AttemptCount { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime DispatchedAtUtc { get; private set; }

    private WebhookDeliveryLog() : base(WebhookDeliveryId.New()) { }

    public WebhookDeliveryLog(
        WebhookSubscriptionId subscriptionId,
        string eventType,
        string payload,
        int? statusCode,
        bool isSuccess,
        int attemptCount,
        string? errorMessage,
        DateTime dispatchedAtUtc) : base(WebhookDeliveryId.New())
    {
        SubscriptionId = subscriptionId;
        EventType = eventType;
        Payload = payload;
        StatusCode = statusCode;
        IsSuccess = isSuccess;
        AttemptCount = attemptCount;
        ErrorMessage = errorMessage;
        DispatchedAtUtc = dispatchedAtUtc;
    }

    public void RecordRetry(int? statusCode, bool isSuccess, string? errorMessage, DateTime dispatchedAtUtc)
    {
        AttemptCount++;
        StatusCode = statusCode;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        DispatchedAtUtc = dispatchedAtUtc;
    }
}
