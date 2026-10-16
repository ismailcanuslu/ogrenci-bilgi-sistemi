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

public static class SendBroadcastSmsNotification
{
    public sealed record Command(
        IReadOnlyList<string> Recipients,
        string Message)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        int TotalRecipients,
        int SentCount,
        int FailedCount,
        IReadOnlyList<RecipientResultDto> Results);

    public sealed record RecipientResultDto(
        string Recipient,
        Guid? NotificationId,
        string Status,
        string? FailureReason = null);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Recipients)
                .NotEmpty()
                .Must(r => r.Count > 0 && r.Count <= 1000)
                .WithMessage("Alıcı sayısı 1 ile 1000 arasında olmalıdır.");

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
            var bodyResult = Body.Create(request.Message, isHtml: false);
            if (!bodyResult.IsSuccess)
            {
                return ServiceResult<Response>.BadRequest(bodyResult.Errors.ToArray());
            }

            var body = bodyResult.Value!;
            var results = new List<RecipientResultDto>();
            var notificationsToAdd = new List<Notification>();
            var sentCount = 0;
            var failedCount = 0;

            foreach (var recipient in request.Recipients)
            {
                var phoneResult = PhoneNumber.Create(recipient);
                if (!phoneResult.IsSuccess)
                {
                    failedCount++;
                    results.Add(new RecipientResultDto(
                        recipient,
                        null,
                        "Failed",
                        "Geçersiz telefon numarası formatı"));
                    continue;
                }

                var phone = phoneResult.Value!;
                var now = DateTimeOffset.UtcNow;
                var notification = Notification.CreateSms(phone, body, now);
                notificationsToAdd.Add(notification);

                try
                {
                    await smsSender.SendSmsAsync(phone, body, cancellationToken);
                    notification.MarkAsSent(DateTimeOffset.UtcNow);
                    sentCount++;
                    results.Add(new RecipientResultDto(
                        recipient,
                        notification.Id.Value,
                        "Sent"));
                }
                catch (Exception ex)
                {
                    notification.MarkAsFailed(ex.Message);
                    failedCount++;
                    results.Add(new RecipientResultDto(
                        recipient,
                        notification.Id.Value,
                        "Failed",
                        ex.Message));
                }
            }

            if (notificationsToAdd.Count > 0)
            {
                await notificationRepository.AddRangeAsync(notificationsToAdd, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var response = new Response(
                request.Recipients.Count,
                sentCount,
                failedCount,
                results);

            return ServiceResult<Response>.Ok(response);
        }
    }
}
