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

public class SendBroadcastSmsNotificationHandlerTests
{
    private readonly INotificationRepository _repository = Substitute.For<INotificationRepository>();
    private readonly ISmsSender _smsSender = Substitute.For<ISmsSender>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_WhenAllRecipientsValid_ShouldSendToAllAndReturnOk()
    {
        // Arrange
        var recipients = new List<string> { "+905551112233", "+905552223344" };
        var handler = new SendBroadcastSmsNotification.Handler(_repository, _smsSender, _unitOfWork);
        var command = new SendBroadcastSmsNotification.Command(
            recipients,
            "Sistem 14:00'te bakıma girecektir.");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Data!.TotalRecipients);
        Assert.Equal(2, result.Data.SentCount);
        Assert.Equal(0, result.Data.FailedCount);

        await _smsSender.Received(2).SendSmsAsync(
            Arg.Any<PhoneNumber>(),
            Arg.Any<Body>(),
            Arg.Any<CancellationToken>());

        await _repository.Received(1).AddRangeAsync(
            Arg.Is<IEnumerable<Notification>>(n => n.Count() == 2),
            Arg.Any<CancellationToken>());

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSomeRecipientsInvalid_ShouldRecordValidationFailure()
    {
        // Arrange
        var recipients = new List<string> { "+905551112233", "gecersiz-no" };
        var handler = new SendBroadcastSmsNotification.Handler(_repository, _smsSender, _unitOfWork);
        var command = new SendBroadcastSmsNotification.Command(
            recipients,
            "Acil bakım uyarısı");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.TotalRecipients);
        Assert.Equal(1, result.Data.SentCount);
        Assert.Equal(1, result.Data.FailedCount);

        Assert.Equal("Sent", result.Data.Results[0].Status);
        Assert.Equal("Failed", result.Data.Results[1].Status);
        Assert.Equal("Geçersiz telefon numarası formatı", result.Data.Results[1].FailureReason);
    }

    [Fact]
    public async Task Handle_WhenSmsDeliveryThrowsException_ShouldMarkIndividualRecipientFailed()
    {
        // Arrange
        var recipients = new List<string> { "+905551112233" };

        _smsSender.SendSmsAsync(Arg.Any<PhoneNumber>(), Arg.Any<Body>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Gateway timeout 504"));

        var handler = new SendBroadcastSmsNotification.Handler(_repository, _smsSender, _unitOfWork);
        var command = new SendBroadcastSmsNotification.Command(
            recipients,
            "Sistem devre dışı kaldı.");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data!.SentCount);
        Assert.Equal(1, result.Data.FailedCount);
        Assert.Equal("Failed", result.Data.Results[0].Status);
        Assert.Contains("Gateway timeout", result.Data.Results[0].FailureReason);
    }
}
