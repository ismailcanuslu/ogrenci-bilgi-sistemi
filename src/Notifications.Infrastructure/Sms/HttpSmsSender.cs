using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Domain.ValueObjects;
using Notifications.Infrastructure.Options;

namespace Notifications.Infrastructure.Sms;

public sealed class HttpSmsSender(
    HttpClient httpClient,
    IOptions<NotificationOptions> options,
    ILogger<HttpSmsSender> logger) : ISmsSender
{
    public async Task SendSmsAsync(
        PhoneNumber to,
        Body body,
        CancellationToken cancellationToken = default)
    {
        var smsOptions = options.Value.Sms;
        if (string.IsNullOrWhiteSpace(smsOptions.EndpointUrl))
        {
            throw new InvalidOperationException("SMS HTTP gateway endpoint URL is not configured.");
        }

        var requestPayload = new
        {
            To = to.Value,
            Body = body.Value,
            Header = smsOptions.SenderHeader
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, smsOptions.EndpointUrl)
        {
            Content = JsonContent.Create(requestPayload)
        };

        if (!string.IsNullOrWhiteSpace(smsOptions.ApiKey))
        {
            request.Headers.Add("X-Api-Key", smsOptions.ApiKey);
        }

        if (!string.IsNullOrWhiteSpace(smsOptions.ApiSecret))
        {
            request.Headers.Add("X-Api-Secret", smsOptions.ApiSecret);
        }

        logger.LogInformation(
            "Sending SMS via HTTP Gateway to {To} with header {Header}",
            to.Value,
            smsOptions.SenderHeader);

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(
                "SMS delivery failed via HTTP gateway. StatusCode: {StatusCode}, Body: {Body}",
                response.StatusCode,
                errorContent);

            throw new HttpRequestException(
                $"SMS delivery failed with status code {(int)response.StatusCode}: {errorContent}");
        }

        logger.LogInformation("SMS successfully delivered to {To} via HTTP Gateway", to.Value);
    }
}
