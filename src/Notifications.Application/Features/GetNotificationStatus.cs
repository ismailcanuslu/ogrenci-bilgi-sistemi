using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Domain.Errors;
using Notifications.Domain.Notifications;

namespace Notifications.Application.Features;

public static class GetNotificationStatus
{
    public sealed record Query(Guid Id) : IQuery<ServiceResult<Response>>;

    public sealed record Response(
        Guid Id,
        string Type,
        string Recipient,
        string? Subject,
        string Status,
        int RetryCount,
        string? FailureReason,
        DateTimeOffset CreatedAt,
        DateTimeOffset? SentAt);

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }

    public sealed class Handler(INotificationRepository notificationRepository)
        : IQueryHandler<Query, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var notification = await notificationRepository.GetByIdAsync(
                new NotificationId(request.Id),
                cancellationToken);

            if (notification is null)
            {
                return ServiceResult<Response>.NotFound(NotificationErrors.NotFound);
            }

            return ServiceResult<Response>.Ok(
                new Response(
                    notification.Id.Value,
                    notification.Type.ToString(),
                    notification.Recipient,
                    notification.Subject?.Value,
                    notification.Status.ToString(),
                    notification.RetryCount,
                    notification.FailureReason,
                    notification.CreatedAt,
                    notification.SentAt));
        }
    }
}
