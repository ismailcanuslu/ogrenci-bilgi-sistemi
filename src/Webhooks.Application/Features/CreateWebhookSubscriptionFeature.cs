using BuildingBlocks.Application.Abstractions.Time;
using BuildingBlocks.Application.Results;
using Webhooks.Application.Abstractions;
using Webhooks.Domain;

namespace Webhooks.Application.Features;

public sealed record CreateWebhookSubscriptionRequest(
    string TargetUrl,
    string Description,
    List<string> EventTypes,
    string? CustomSecret = null);

public sealed record WebhookSubscriptionDto(
    Guid Id,
    string TargetUrl,
    string Description,
    List<string> EventTypes,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? SecretRotatedAtUtc,
    string? Secret = null);

public static class CreateWebhookSubscriptionFeature
{
    public static async Task<Result<WebhookSubscriptionDto>> HandleAsync(
        CreateWebhookSubscriptionRequest request,
        IWebhookRepository repository,
        IDateTimeProvider dateTimeProvider,
        CancellationToken ct = default)
    {
        try
        {
            var subscription = WebhookSubscription.Create(
                request.TargetUrl,
                request.Description,
                request.EventTypes,
                dateTimeProvider.UtcNow.UtcDateTime,
                request.CustomSecret);

            await repository.AddAsync(subscription, ct).ConfigureAwait(false);

            return Result<WebhookSubscriptionDto>.Success(new WebhookSubscriptionDto(
                subscription.Id.Value,
                subscription.TargetUrl,
                subscription.Description,
                subscription.EventTypes,
                subscription.IsActive,
                subscription.CreatedAtUtc,
                subscription.SecretRotatedAtUtc,
                subscription.Secret));
        }
        catch (ArgumentException)
        {
            return Result<WebhookSubscriptionDto>.Failure(new Error(WebhookErrors.InvalidUrl.Code, ErrorType.Validation));
        }
    }
}
