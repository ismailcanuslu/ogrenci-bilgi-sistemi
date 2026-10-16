using BuildingBlocks.Application.Results;
using MediatR;
using Notifications.Application.Abstractions.Templates;
using Notifications.Application.Features;
using Notifications.Infrastructure.Email;
using NSubstitute;
using Xunit;

namespace UniversalSystem.UnitTests.Notifications;

public class EmailNotificationServiceTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly ITemplateRenderer _renderer = Substitute.For<ITemplateRenderer>();

    [Fact]
    public async Task SendOtpEmailAsync_ShouldRenderTemplateAndDispatchCommand()
    {
        // Arrange
        _renderer.RenderAsync(
            Arg.Any<string>(),
            Arg.Any<IReadOnlyDictionary<string, string>>(),
            Arg.Any<CancellationToken>())
            .Returns("<html>Rendered OTP Body</html>");

        var response = new SendEmailNotification.Response(
            Guid.NewGuid(),
            "student@uni.edu",
            "Doğrulama Kodunuz: 050970",
            "Sent",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        _mediator.Send(
            Arg.Any<SendEmailNotification.Command>(),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<SendEmailNotification.Response>.Created(response));

        var service = new EmailNotificationService(_mediator, _renderer);

        // Act
        var result = await service.SendOtpEmailAsync(
            to: "student@uni.edu",
            otpCode: "050970",
            expiresInMinutes: 5);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal("050970", result.Data!.Subject.Split(':').Last().Trim());

        await _renderer.Received(1).RenderAsync(
            Arg.Any<string>(),
            Arg.Is<IReadOnlyDictionary<string, string>>(d => d["OtpCode"] == "050970"),
            Arg.Any<CancellationToken>());

        await _mediator.Received(1).Send(
            Arg.Is<SendEmailNotification.Command>(c => c.To == "student@uni.edu" && c.IsHtml),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendActionEmailAsync_ShouldRenderTemplateWithButtonAndDispatchCommand()
    {
        // Arrange
        _renderer.RenderAsync(
            Arg.Any<string>(),
            Arg.Any<IReadOnlyDictionary<string, string>>(),
            Arg.Any<CancellationToken>())
            .Returns("<html>Rendered Action Button Body</html>");

        var response = new SendEmailNotification.Response(
            Guid.NewGuid(),
            "student@uni.edu",
            "Parola Sıfırlama",
            "Sent",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        _mediator.Send(
            Arg.Any<SendEmailNotification.Command>(),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<SendEmailNotification.Response>.Created(response));

        var service = new EmailNotificationService(_mediator, _renderer);

        // Act
        var result = await service.SendActionEmailAsync(
            to: "student@uni.edu",
            subject: "Parola Sıfırlama",
            heading: "Parolanızı Sıfırlayın",
            message: "Aşağıdaki butona tıklayarak devam edin.",
            buttonText: "Parolamı Sıfırla",
            buttonUrl: "https://obs.uni.edu.tr/reset",
            expiresInMinutes: 15);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(201, result.StatusCode);

        await _renderer.Received(1).RenderAsync(
            Arg.Any<string>(),
            Arg.Is<IReadOnlyDictionary<string, string>>(d =>
                d["ButtonUrl"] == "https://obs.uni.edu.tr/reset" &&
                d["ButtonText"] == "Parolamı Sıfırla"),
            Arg.Any<CancellationToken>());
    }
}
