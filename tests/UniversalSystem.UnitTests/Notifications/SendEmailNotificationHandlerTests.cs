using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Application.Features;
using Notifications.Domain.Notifications;
using Notifications.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace UniversalSystem.UnitTests.Notifications;

public class SendEmailNotificationHandlerTests
{
    private readonly INotificationRepository _repository = Substitute.For<INotificationRepository>();
    private readonly IEmailSender _emailSender = Substitute.For<IEmailSender>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_WithValidCommand_ShouldSendEmailAndReturnCreated()
    {
        // Arrange
        var handler = new SendEmailNotification.Handler(
            _repository,
            _emailSender,
            _unitOfWork);

        var command = new SendEmailNotification.Command(
            "student@uni.edu",
            "Ders Kaydı",
            "<p>Kaydınız başarıyla tamamlandı.</p>",
            IsHtml: true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("student@uni.edu", result.Data.Recipient);
        Assert.Equal("Ders Kaydı", result.Data.Subject);
        Assert.Equal("Sent", result.Data.Status);

        await _repository.Received(1).AddAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
        await _emailSender.Received(1).SendEmailAsync(
            Arg.Any<EmailAddress>(),
            Arg.Any<Subject>(),
            Arg.Any<Body>(),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var handler = new SendEmailNotification.Handler(
            _repository,
            _emailSender,
            _unitOfWork);

        var command = new SendEmailNotification.Command(
            "invalid-email",
            "Başlık",
            "İçerik");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Notifications.InvalidEmail", result.Errors.First().Code);

        await _emailSender.DidNotReceive().SendEmailAsync(
            Arg.Any<EmailAddress>(),
            Arg.Any<Subject>(),
            Arg.Any<Body>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSenderThrowsException_ShouldMarkAsFailedAndReturnFailure()
    {
        // Arrange
        _emailSender.SendEmailAsync(
            Arg.Any<EmailAddress>(),
            Arg.Any<Subject>(),
            Arg.Any<Body>(),
            Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("SMTP connection refused"));

        var handler = new SendEmailNotification.Handler(
            _repository,
            _emailSender,
            _unitOfWork);

        var command = new SendEmailNotification.Command(
            "student@uni.edu",
            "Başlık",
            "İçerik");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(500, result.StatusCode);
        Assert.Equal("Notifications.DeliveryFailed", result.Errors.First().Code);

        _repository.Received(1).Update(Arg.Is<Notification>(n => n.Status == NotificationStatus.Failed));
        await _unitOfWork.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
