using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifications.Domain.ValueObjects;
using Notifications.Infrastructure.Options;
using Notifications.Infrastructure.Sms;
using NSubstitute;
using Xunit;

namespace UniversalSystem.UnitTests.Notifications;

public class HttpSmsSenderTests
{
    private readonly ILogger<HttpSmsSender> _logger = Substitute.For<ILogger<HttpSmsSender>>();

    private sealed class MockHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public HttpRequestMessage? CapturedRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CapturedRequest = request;
            return Task.FromResult(response);
        }
    }

    [Fact]
    public async Task SendSmsAsync_WhenValidConfigAndSuccessResponse_ShouldSendHttpRequestWithHeaders()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
        var httpClient = new HttpClient(handler);

        var options = Substitute.For<IOptions<NotificationOptions>>();
        options.Value.Returns(new NotificationOptions
        {
            Sms = new SmsOptions
            {
                EndpointUrl = "https://sms.provider.com/v1/send",
                SenderHeader = "TESTGATEWAY",
                ApiKey = "api-secret-key-123"
            }
        });

        var sender = new HttpSmsSender(httpClient, options, _logger);
        var phone = PhoneNumber.Create("+905551234567").Value!;
        var body = Body.Create("Test SMS Mesajı", false).Value!;

        // Act
        await sender.SendSmsAsync(phone, body);

        // Assert
        Assert.NotNull(handler.CapturedRequest);
        Assert.Equal("https://sms.provider.com/v1/send", handler.CapturedRequest.RequestUri?.ToString());
        Assert.True(handler.CapturedRequest.Headers.Contains("X-Api-Key"));
        Assert.Equal("api-secret-key-123", handler.CapturedRequest.Headers.GetValues("X-Api-Key").First());
    }

    [Fact]
    public async Task SendSmsAsync_WhenEndpointUrlMissing_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
        var httpClient = new HttpClient(handler);

        var options = Substitute.For<IOptions<NotificationOptions>>();
        options.Value.Returns(new NotificationOptions
        {
            Sms = new SmsOptions
            {
                EndpointUrl = null
            }
        });

        var sender = new HttpSmsSender(httpClient, options, _logger);
        var phone = PhoneNumber.Create("+905551234567").Value!;
        var body = Body.Create("Mesaj", false).Value!;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sender.SendSmsAsync(phone, body));
    }

    [Fact]
    public async Task SendSmsAsync_WhenHttpError_ShouldThrowHttpRequestException()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Gateway error")
        });
        var httpClient = new HttpClient(handler);

        var options = Substitute.For<IOptions<NotificationOptions>>();
        options.Value.Returns(new NotificationOptions
        {
            Sms = new SmsOptions
            {
                EndpointUrl = "https://sms.provider.com/v1/send"
            }
        });

        var sender = new HttpSmsSender(httpClient, options, _logger);
        var phone = PhoneNumber.Create("+905551234567").Value!;
        var body = Body.Create("Mesaj", false).Value!;

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            sender.SendSmsAsync(phone, body));
    }
}
