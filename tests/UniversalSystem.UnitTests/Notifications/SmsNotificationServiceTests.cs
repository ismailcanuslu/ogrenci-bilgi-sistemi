using BuildingBlocks.Application.Results;
using MediatR;
using Notifications.Application.Features;
using Notifications.Infrastructure.Sms;
using NSubstitute;
using Xunit;

namespace UniversalSystem.UnitTests.Notifications;

public class SmsNotificationServiceTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();

    [Fact]
    public async Task SendOtpSmsAsync_ShouldFormatExactMessageAndDefaultTo15Minutes()
    {
        // Arrange
        var response = new SendSmsNotification.Response(
            Guid.NewGuid(),
            "+905551234567",
            "Sent",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        _mediator.Send(
            Arg.Any<SendSmsNotification.Command>(),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<SendSmsNotification.Response>.Created(response));

        var service = new SmsNotificationService(_mediator);

        // Act
        var result = await service.SendOtpSmsAsync(
            to: "+905551234567",
            otpCode: "050970");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal("+905551234567", result.Data!.Recipient);

        await _mediator.Received(1).Send(
            Arg.Is<SendSmsNotification.Command>(c =>
                c.To == "+905551234567" &&
                c.Message == "Doğrulama kodu: 050970. Kod 15 dakika boyunca geçerlidir."),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendSmsAsync_ShouldDispatchGenericSmsCommand()
    {
        // Arrange
        var response = new SendSmsNotification.Response(
            Guid.NewGuid(),
            "+905559876543",
            "Sent",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        _mediator.Send(
            Arg.Any<SendSmsNotification.Command>(),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<SendSmsNotification.Response>.Created(response));

        var service = new SmsNotificationService(_mediator);

        // Act
        var result = await service.SendSmsAsync(
            to: "+905559876543",
            message: "Sistem planlı bakım nedeniyle kapatılmıştır.");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(201, result.StatusCode);

        await _mediator.Received(1).Send(
            Arg.Is<SendSmsNotification.Command>(c =>
                c.To == "+905559876543" &&
                c.Message == "Sistem planlı bakım nedeniyle kapatılmıştır."),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendBroadcastSmsAsync_ShouldDispatchBroadcastSmsCommand()
    {
        // Arrange
        var recipients = new List<string> { "+905551112233", "+905552223344" };
        var response = new SendBroadcastSmsNotification.Response(
            2,
            2,
            0,
            new List<SendBroadcastSmsNotification.RecipientResultDto>
            {
                new("+905551112233", Guid.NewGuid(), "Sent"),
                new("+905552223344", Guid.NewGuid(), "Sent")
            });

        _mediator.Send(
            Arg.Any<SendBroadcastSmsNotification.Command>(),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<SendBroadcastSmsNotification.Response>.Ok(response));

        var service = new SmsNotificationService(_mediator);

        // Act
        var result = await service.SendBroadcastSmsAsync(
            recipients,
            "Sistem saat 22:00'de bakıma alınacaktır.");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Data!.TotalRecipients);
        Assert.Equal(2, result.Data.SentCount);

        await _mediator.Received(1).Send(
            Arg.Is<SendBroadcastSmsNotification.Command>(c =>
                c.Recipients.Count == 2 &&
                c.Message == "Sistem saat 22:00'de bakıma alınacaktır."),
            Arg.Any<CancellationToken>());
    }
}
