using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Domain.Errors;
using Notifications.Domain.Notifications;

namespace Notifications.Application.Features;

public static class CancelNotification
{
    public sealed record Command(Guid Id) : ICommand<ServiceResult>;

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
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult>
    {
        public async Task<ServiceResult> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var notification = await notificationRepository.GetByIdAsync(
                new NotificationId(request.Id),
                cancellationToken);

            if (notification is null)
            {
                return ServiceResult.NotFound(NotificationErrors.NotFound);
            }

            var cancelResult = notification.Cancel();
            if (!cancelResult.IsSuccess)
            {
                return ServiceResult.Conflict(cancelResult.Errors.ToArray());
            }

            notificationRepository.Update(notification);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.NoContent();
        }
    }
}
