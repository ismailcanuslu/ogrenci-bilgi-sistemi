using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Domain.Errors;
using Notifications.Domain.Notifications;
using Notifications.Domain.ValueObjects;

namespace Notifications.Application.Features;

public static class SendSmsNotification
{
    public sealed record Command(
        string To,
        string Message)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        Guid Id,
        string Recipient,
        string Status,
        DateTimeOffset CreatedAt,
        DateTimeOffset? SentAt,
        string? FailureReason = null);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.To)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.Message)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }

    public sealed class Handler(
        INotificationRepository notificationRepository,
        ISmsSender smsSender,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var phoneResult = PhoneNumber.Create(request.To);
            if (!phoneResult.IsSuccess)
            {
                return ServiceResult<Response>.BadRequest(phoneResult.Errors.ToArray());
            }

            var bodyResult = Body.Create(request.Message, isHtml: false);
            if (!bodyResult.IsSuccess)
            {
                return ServiceResult<Response>.BadRequest(bodyResult.Errors.ToArray());
            }

            var now = DateTimeOffset.UtcNow;
            var notification = Notification.CreateSms(
                phoneResult.Value!,
                bodyResult.Value!,
                now);

            await notificationRepository.AddAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                await smsSender.SendSmsAsync(
                    phoneResult.Value!,
                    bodyResult.Value!,
                    cancellationToken);

                notification.MarkAsSent(DateTimeOffset.UtcNow);
            }
            catch (Exception ex)
            {
                notification.MarkAsFailed(ex.Message);
            }

            notificationRepository.Update(notification);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new Response(
                notification.Id.Value,
                notification.Recipient,
                notification.Status.ToString(),
                notification.CreatedAt,
                notification.SentAt,
                notification.FailureReason);

            return notification.Status == NotificationStatus.Sent
                ? ServiceResult<Response>.Created(response)
                : ServiceResult<Response>.Failure(NotificationErrors.SmsDeliveryFailed);
        }
    }
}
