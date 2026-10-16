using System.Security.Cryptography;
using BuildingBlocks.Domain.Primitives;

namespace Webhooks.Domain;

public sealed class WebhookSubscription : AggregateRoot<WebhookSubscriptionId>
{
    public string TargetUrl { get; private set; } = string.Empty;
    public string Secret { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public List<string> EventTypes { get; private set; } = new();
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? SecretRotatedAtUtc { get; private set; }

    private WebhookSubscription() : base(WebhookSubscriptionId.New()) { }

    private WebhookSubscription(
        WebhookSubscriptionId id,
        string targetUrl,
        string secret,
        string description,
        IEnumerable<string> eventTypes,
        DateTime createdAtUtc) : base(id)
    {
        TargetUrl = targetUrl;
        Secret = secret;
        Description = description;
        EventTypes = eventTypes.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public static WebhookSubscription Create(
        string targetUrl,
        string description,
        IEnumerable<string> eventTypes,
        DateTime nowUtc,
        string? customSecret = null)
    {
        if (string.IsNullOrWhiteSpace(targetUrl))
            throw new ArgumentException("Webhook hedef URL'i boş olamaz.", nameof(targetUrl));

        if (!Uri.TryCreate(targetUrl, UriKind.Absolute, out var uri) || 
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Geçerli bir HTTP veya HTTPS webhook URL'i girilmelidir.", nameof(targetUrl));
        }

        var secret = string.IsNullOrWhiteSpace(customSecret) ? GenerateSecureSecret() : customSecret;
        return new WebhookSubscription(WebhookSubscriptionId.New(), targetUrl, secret, description, eventTypes, nowUtc);
    }

    public void UpdateTargetUrl(string newUrl)
    {
        if (string.IsNullOrWhiteSpace(newUrl))
            throw new ArgumentException("Webhook hedef URL'i boş olamaz.", nameof(newUrl));

        if (!Uri.TryCreate(newUrl, UriKind.Absolute, out var uri) || 
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Geçerli bir HTTP veya HTTPS webhook URL'i girilmelidir.", nameof(newUrl));
        }

        TargetUrl = newUrl;
    }

    public void UpdateEventTypes(IEnumerable<string> newEvents)
    {
        EventTypes = newEvents.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    public void RotateSecret(DateTime nowUtc)
    {
        Secret = GenerateSecureSecret();
        SecretRotatedAtUtc = nowUtc;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public bool SubscribesTo(string eventType)
    {
        if (!IsActive) return false;
        if (EventTypes.Contains("*", StringComparer.OrdinalIgnoreCase)) return true;
        return EventTypes.Contains(eventType, StringComparer.OrdinalIgnoreCase);
    }

    public static string GenerateSecureSecret()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
