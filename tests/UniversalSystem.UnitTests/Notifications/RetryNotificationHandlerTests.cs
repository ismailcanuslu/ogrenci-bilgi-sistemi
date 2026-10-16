using BuildingBlocks.Application.Abstractions.Persistence;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Application.Features;
using Notifications.Domain.Errors;
using Notifications.Domain.Notifications;
using Notifications.Domain.ValueObjects;
using NSubstitute;
using Xunit;

namespace UniversalSystem.UnitTests.Notifications;

public class RetryNotificationHandlerTests
{
    private readonly INotificationRepository _repository = Substitute.For<INotificationRepository>();
    private readonly IEmailSender _emailSender = Substitute.For<IEmailSender>();
    private readonly ISmsSender _smsSender = Substitute.For<ISmsSender>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_WhenFailedEmailNotification_ShouldRetryAndMarkAsSent()
    {
        // Arrange
        var notification = Notification.CreateEmail(
            EmailAddress.Create("student@uni.edu").Value!,
            Subject.Create("Önemli Bilgilendirme").Value!,
            Body.Create("Lütfen kayıt yenileyin", false).Value!,
            DateTimeOffset.UtcNow);

        notification.MarkAsFailed("SMTP timeout");

        _repository.GetByIdAsync(Arg.Any<NotificationId>(), Arg.Any<CancellationToken>())
            .Returns(notification);

        var handler = new RetryNotification.Handler(_repository, _emailSender, _smsSender, _unitOfWork);
        var command = new RetryNotification.Command(notification.Id.Value);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Sent", result.Data!.Status);
        Assert.Equal(1, result.Data.RetryCount);

        await _emailSender.Received(1).SendEmailAsync(
            Arg.Any<EmailAddress>(),
            Arg.Any<Subject>(),
            Arg.Any<Body>(),
            Arg.Any<CancellationToken>());

        _repository.Received(1).Update(notification);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFailedSmsNotification_ShouldRetryViaSmsSender()
    {
        // Arrange
        var notification = Notification.CreateSms(
            PhoneNumber.Create("+905551234567").Value!,
            Body.Create("Sınav saati değişti: 14:00", false).Value!,
            DateTimeOffset.UtcNow);

        notification.MarkAsFailed("Gateway unreachable");

        _repository.GetByIdAsync(Arg.Any<NotificationId>(), Arg.Any<CancellationToken>())
            .Returns(notification);

        var handler = new RetryNotification.Handler(_repository, _emailSender, _smsSender, _unitOfWork);
        var command = new RetryNotification.Command(notification.Id.Value);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Sent", result.Data!.Status);
        Assert.Equal(1, result.Data.RetryCount);

        await _smsSender.Received(1).SendSmsAsync(
            Arg.Any<PhoneNumber>(),
            Arg.Any<Body>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNotificationNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<NotificationId>(), Arg.Any<CancellationToken>())
            .Returns((Notification?)null);

        var handler = new RetryNotification.Handler(_repository, _emailSender, _smsSender, _unitOfWork);
        var command = new RetryNotification.Command(Guid.NewGuid());

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
            Subject.Create("Konu").Value!,
            Body.Create("İçerik", false).Value!,
            DateTimeOffset.UtcNow);

        notification.MarkAsSent(DateTimeOffset.UtcNow);

        _repository.GetByIdAsync(Arg.Any<NotificationId>(), Arg.Any<CancellationToken>())
            .Returns(notification);

        var handler = new RetryNotification.Handler(_repository, _emailSender, _smsSender, _unitOfWork);
        var command = new RetryNotification.Command(notification.Id.Value);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(409, result.StatusCode);
        Assert.Equal(NotificationErrors.AlreadySent.Code, result.Errors.First().Code);
    }
}
