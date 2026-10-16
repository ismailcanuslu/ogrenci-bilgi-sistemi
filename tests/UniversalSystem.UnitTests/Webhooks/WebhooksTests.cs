using System.Net;
using BuildingBlocks.Application.Abstractions.Time;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Webhooks.Application.Abstractions;
using Webhooks.Domain;
using Webhooks.Infrastructure.Services;
using Xunit;

namespace UniversalSystem.UnitTests.Webhooks;

public class WebhooksTests
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public WebhooksTests()
    {
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _dateTimeProvider.UtcNow.Returns(new DateTimeOffset(2026, 10, 13, 12, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void WebhookSubscription_Create_ShouldInitializeWithValidStateAndSecret()
    {
        // Act
        var sub = WebhookSubscription.Create(
            "https://api.example.com/obs-events",
            "Example webhook",
            new[] { "user.created", "file.uploaded" },
            _dateTimeProvider.UtcNow.UtcDateTime);

        // Assert
        Assert.Equal("https://api.example.com/obs-events", sub.TargetUrl);
        Assert.Equal("Example webhook", sub.Description);
        Assert.True(sub.IsActive);
        Assert.False(string.IsNullOrWhiteSpace(sub.Secret));
        Assert.Equal(64, sub.Secret.Length); // 32 bytes hex
        Assert.True(sub.SubscribesTo("user.created"));
        Assert.False(sub.SubscribesTo("user.deleted"));
    }

    [Fact]
    public void WebhookSubscription_RotateSecret_ShouldGenerateNewSecretAndUpdateTimestamp()
    {
        // Arrange
        var sub = WebhookSubscription.Create(
            "https://api.example.com/obs-events",
            "Example webhook",
            new[] { "*" },
            _dateTimeProvider.UtcNow.UtcDateTime);
        var oldSecret = sub.Secret;
        var rotationTime = _dateTimeProvider.UtcNow.UtcDateTime.AddDays(30);

        // Act
        sub.RotateSecret(rotationTime);

        // Assert
        Assert.NotEqual(oldSecret, sub.Secret);
        Assert.Equal(rotationTime, sub.SecretRotatedAtUtc);
        Assert.True(sub.SubscribesTo("any.random.event")); // Wildcard test
    }

    [Fact]
    public void HmacWebhookSigner_ShouldComputeAndVerifySignatureSuccessfully()
    {
        // Arrange
        var signer = new HmacWebhookSigner();
        var secret = "secret_key_123456789012345678901234";
        var payload = "{\"event\":\"user.created\",\"userId\":\"12345\"}";

        // Act
        var signature = signer.ComputeSignature(payload, secret);
        var isValid = signer.VerifySignature(payload, secret, signature);

        // Assert
        Assert.StartsWith("sha256=", signature);
        Assert.True(isValid);
    }

    [Fact]
    public void HmacWebhookSigner_ShouldFailVerification_WhenPayloadOrSecretDiffers()
    {
        // Arrange
        var signer = new HmacWebhookSigner();
        var secret = "secret_key_123456789012345678901234";
        var payload = "{\"event\":\"user.created\",\"userId\":\"12345\"}";
        var signature = signer.ComputeSignature(payload, secret);

        // Act
        var isValidWithDifferentPayload = signer.VerifySignature(payload + "tampered", secret, signature);
        var isValidWithDifferentSecret = signer.VerifySignature(payload, "wrong_secret", signature);

        // Assert
        Assert.False(isValidWithDifferentPayload);
        Assert.False(isValidWithDifferentSecret);
    }

    [Fact]
    public async Task WebhookDispatcher_DispatchAsync_ShouldSendPayloadWithCorrectHeaders()
    {
        // Arrange
        var signer = new HmacWebhookSigner();
        var repo = Substitute.For<IWebhookRepository>();
        var sub = WebhookSubscription.Create(
            "https://receiver.example.com/webhook",
            "Test receiver",
            new[] { "payment.completed" },
            _dateTimeProvider.UtcNow.UtcDateTime);

        HttpRequestMessage? capturedRequest = null;
        var mockHandler = new TestHttpMessageHandler(async req =>
        {
            capturedRequest = req;
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var httpClient = new HttpClient(mockHandler);
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient("WebhooksClient").Returns(httpClient);

        var dispatcher = new WebhookDispatcher(
            httpClientFactory,
            signer,
            repo,
            _dateTimeProvider,
            NullLogger<WebhookDispatcher>.Instance);

        var payload = "{\"paymentId\":999}";

        // Act
        var result = await dispatcher.DispatchAsync(sub, "payment.completed", payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(1, result.AttemptCount);
        Assert.NotNull(capturedRequest);
        Assert.True(capturedRequest!.Headers.Contains("X-Hub-Signature-256"));
        Assert.Equal("payment.completed", capturedRequest.Headers.GetValues("X-Webhook-Event").First());

        await repo.Received(1).AddDeliveryLogAsync(
            Arg.Is<WebhookDeliveryLog>(l => l.IsSuccess && l.EventType == "payment.completed"),
            Arg.Any<CancellationToken>());
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public TestHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handler(request);
        }
    }
}
