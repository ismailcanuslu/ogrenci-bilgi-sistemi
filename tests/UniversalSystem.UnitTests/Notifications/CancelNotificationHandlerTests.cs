using BuildingBlocks.Application.Abstractions.Persistence;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Application.Features;
using Notifications.Domain.Errors;
using Notifications.Domain.Notifications;
using Notifications.Domain.ValueObjects;
using NSubstitute;
using Xunit;

namespace UniversalSystem.UnitTests.Notifications;

public class CancelNotificationHandlerTests
{
    private readonly INotificationRepository _repository = Substitute.For<INotificationRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_WhenPendingNotification_ShouldCancelAndReturnNoContent()
    {
        // Arrange
        var notification = Notification.CreateEmail(
            EmailAddress.Create("student@uni.edu").Value!,
            Subject.Create("Bekleyen Bildirim").Value!,
            Body.Create("İçerik", false).Value!,
            DateTimeOffset.UtcNow);

        _repository.GetByIdAsync(Arg.Any<NotificationId>(), Arg.Any<CancellationToken>())
            .Returns(notification);

        var handler = new CancelNotification.Handler(_repository, _unitOfWork);
        var command = new CancelNotification.Command(notification.Id.Value);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(204, result.StatusCode);
        Assert.Equal(NotificationStatus.Cancelled, notification.Status);

        _repository.Received(1).Update(notification);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNotificationNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<NotificationId>(), Arg.Any<CancellationToken>())
            .Returns((Notification?)null);

        var handler = new CancelNotification.Handler(_repository, _unitOfWork);
        var command = new CancelNotification.Command(Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal(NotificationErrors.NotFound.Code, result.Errors.First().Code);
    }

    [Fact]
    public async Task Handle_WhenNotificationAlreadySent_ShouldReturnConflict()
    {
        // Arrange
        var notification = Notification.CreateEmail(
            EmailAddress.Create("student@uni.edu").Value!,
            Subject.Create("Gönderilmiş Bildirim").Value!,
            Body.Create("İçerik", false).Value!,
            DateTimeOffset.UtcNow);

        notification.MarkAsSent(DateTimeOffset.UtcNow);

        _repository.GetByIdAsync(Arg.Any<NotificationId>(), Arg.Any<CancellationToken>())
            .Returns(notification);

        var handler = new CancelNotification.Handler(_repository, _unitOfWork);
        var command = new CancelNotification.Command(notification.Id.Value);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(409, result.StatusCode);
        Assert.Equal(NotificationErrors.AlreadySent.Code, result.Errors.First().Code);
    }
}
