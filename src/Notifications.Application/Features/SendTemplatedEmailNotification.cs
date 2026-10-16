using System.Text.Json;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Application.Abstractions.Templates;
using Notifications.Domain.Errors;
using Notifications.Domain.Notifications;
using Notifications.Domain.ValueObjects;

namespace Notifications.Application.Features;

public static class SendTemplatedEmailNotification
{
    public sealed record Command(
        string To,
        string Subject,
        string TemplateCode,
        Dictionary<string, string> Parameters,
        bool IsHtml = true)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        Guid Id,
        string Recipient,
        string Subject,
        string TemplateCode,
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
                .MaximumLength(256);

            RuleFor(x => x.Subject)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.TemplateCode)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Parameters)
                .NotNull();
        }
    }

    public sealed class Handler(
        INotificationRepository notificationRepository,
        ITemplateRenderer templateRenderer,
        IEmailSender emailSender,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var emailResult = EmailAddress.Create(request.To);
            if (!emailResult.IsSuccess)
            {
                return ServiceResult<Response>.BadRequest(emailResult.Errors.ToArray());
            }

            var subjectResult = Subject.Create(request.Subject);
            if (!subjectResult.IsSuccess)
            {
                return ServiceResult<Response>.BadRequest(subjectResult.Errors.ToArray());
            }

            var renderedContent = await templateRenderer.RenderAsync(
                request.TemplateCode,
                request.Parameters,
                cancellationToken);

            var bodyResult = Body.Create(renderedContent, request.IsHtml);
            if (!bodyResult.IsSuccess)
            {
                return ServiceResult<Response>.BadRequest(bodyResult.Errors.ToArray());
            }

            var parametersJson = JsonSerializer.Serialize(request.Parameters);
            var now = DateTimeOffset.UtcNow;

            var notification = Notification.CreateTemplatedEmail(
                emailResult.Value!,
                subjectResult.Value!,
                request.TemplateCode,
                parametersJson,
                bodyResult.Value!,
                now);

            await notificationRepository.AddAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                await emailSender.SendEmailAsync(
                    emailResult.Value!,
                    subjectResult.Value!,
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
                notification.Subject?.Value ?? string.Empty,
                notification.TemplateCode!,
                notification.Status.ToString(),
                notification.CreatedAt,
                notification.SentAt,
                notification.FailureReason);

            return notification.Status == NotificationStatus.Sent
                ? ServiceResult<Response>.Created(response)
                : ServiceResult<Response>.Failure(new Error("Notifications.DeliveryFailed", ErrorType.Failure));
        }
    }
}
