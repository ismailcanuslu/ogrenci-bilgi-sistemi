using BuildingBlocks.Application.Abstractions.Persistence;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Application.Features;
using Notifications.Domain.Notifications;
using Notifications.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace UniversalSystem.UnitTests.Notifications;

public class SendSmsNotificationHandlerTests
{
    private readonly INotificationRepository _repository = Substitute.For<INotificationRepository>();
    private readonly ISmsSender _smsSender = Substitute.For<ISmsSender>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_WithValidCommand_ShouldSendSmsAndReturnCreated()
    {
        // Arrange
        var handler = new SendSmsNotification.Handler(
            _repository,
            _smsSender,
            _unitOfWork);

        var command = new SendSmsNotification.Command(
            "+905551234567",
            "OBS: 123456 kodlu şifrenizle giriş yapabilirsiniz.");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("+905551234567", result.Data.Recipient);
        Assert.Equal("Sent", result.Data.Status);

        await _repository.Received(1).AddAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
        await _smsSender.Received(1).SendSmsAsync(
            Arg.Any<PhoneNumber>(),
            Arg.Any<Body>(),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidPhoneNumber_ShouldReturnBadRequest()
    {
        // Arrange
        var handler = new SendSmsNotification.Handler(
            _repository,
            _smsSender,
            _unitOfWork);

        var command = new SendSmsNotification.Command(
            "invalid-phone",
            "Mesaj");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Notifications.InvalidPhoneNumber", result.Errors.First().Code);

        await _smsSender.DidNotReceive().SendSmsAsync(
            Arg.Any<PhoneNumber>(),
            Arg.Any<Body>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSenderThrowsException_ShouldMarkAsFailedAndReturnFailure()
    {
        // Arrange
        _smsSender.SendSmsAsync(
            Arg.Any<PhoneNumber>(),
            Arg.Any<Body>(),
            Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Gateway timeout"));

        var handler = new SendSmsNotification.Handler(
            _repository,
            _smsSender,
            _unitOfWork);

        var command = new SendSmsNotification.Command(
            "+905551234567",
            "Mesaj");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(500, result.StatusCode);
        Assert.Equal("Notifications.SmsDeliveryFailed", result.Errors.First().Code);

        _repository.Received(1).Update(Arg.Is<Notification>(n => n.Status == NotificationStatus.Failed));
        await _unitOfWork.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
