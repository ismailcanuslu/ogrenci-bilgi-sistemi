using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Domain.Notifications;

namespace Notifications.Application.Features;

public static class GetNotificationsHistory
{
    public sealed record Query(
        int Page = 1,
        int PageSize = 20,
        string? Recipient = null,
        string? Status = null)
        : IQuery<ServiceResult<Response>>;

    public sealed record Response(
        IReadOnlyList<ItemDto> Items,
        int TotalCount,
        int Page,
        int PageSize);

    public sealed record ItemDto(
        Guid Id,
        string Type,
        string Recipient,
        string? Subject,
        string Status,
        int RetryCount,
        DateTimeOffset CreatedAt,
        DateTimeOffset? SentAt,
        string? FailureReason = null);

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);
        }
    }

    public sealed class Handler(INotificationRepository notificationRepository)
        : IQueryHandler<Query, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            NotificationStatus? statusFilter = null;
            if (!string.IsNullOrWhiteSpace(request.Status) &&
                Enum.TryParse<NotificationStatus>(request.Status, ignoreCase: true, out var parsedStatus))
            {
                statusFilter = parsedStatus;
            }

            var (items, totalCount) = await notificationRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.Recipient,
                statusFilter,
                cancellationToken);

            var dtos = items.Select(x => new ItemDto(
                x.Id.Value,
                x.Type.ToString(),
                x.Recipient,
                x.Subject?.Value,
                x.Status.ToString(),
                x.RetryCount,
                x.CreatedAt,
                x.SentAt,
                x.FailureReason))
                .ToList();

            return ServiceResult<Response>.Ok(
                new Response(
                    dtos,
                    totalCount,
                    request.Page,
                    request.PageSize));
        }
    }
}
