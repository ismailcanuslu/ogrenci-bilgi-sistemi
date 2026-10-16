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

public static class RetryNotification
{
    public sealed record Command(Guid Id) : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        Guid Id,
        string Status,
        int RetryCount,
        DateTimeOffset? SentAt,
        string? FailureReason = null);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }

    public sealed class Handler(
        INotificationRepository notificationRepository,
        IEmailSender emailSender,
        ISmsSender smsSender,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var notification = await notificationRepository.GetByIdAsync(
                new NotificationId(request.Id),
                cancellationToken);

            if (notification is null)
            {
                return ServiceResult<Response>.NotFound(NotificationErrors.NotFound);
            }

            if (notification.Status is NotificationStatus.Sent)
            {
                return ServiceResult<Response>.Conflict(NotificationErrors.AlreadySent);
            }

            if (notification.Status is NotificationStatus.Cancelled)
            {
                return ServiceResult<Response>.Conflict(NotificationErrors.AlreadyCancelled);
            }

            notification.RecordRetry();

            try
            {
                if (notification.Type is NotificationType.Email)
                {
                    var email = EmailAddress.Create(notification.Recipient).Value!;
                    var subject = notification.Subject ?? Subject.Create("Bildirim").Value!;
                    await emailSender.SendEmailAsync(
                        email,
                        subject,
                        notification.Body,
                        cancellationToken);
                }
                else if (notification.Type is NotificationType.Sms)
                {
                    var phone = PhoneNumber.Create(notification.Recipient).Value!;
                    await smsSender.SendSmsAsync(
                        phone,
                        notification.Body,
                        cancellationToken);
                }

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
                notification.Status.ToString(),
                notification.RetryCount,
                notification.SentAt,
                notification.FailureReason);

            return notification.Status == NotificationStatus.Sent
                ? ServiceResult<Response>.Ok(response)
                : ServiceResult<Response>.Failure(NotificationErrors.RetryFailed);
        }
    }
}
