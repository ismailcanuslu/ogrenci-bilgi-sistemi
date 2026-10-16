using Microsoft.Extensions.Logging;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Domain.ValueObjects;

namespace Notifications.Infrastructure.Sms;

public sealed class ConsoleSmsSender(ILogger<ConsoleSmsSender> logger) : ISmsSender
{
    public Task SendSmsAsync(
        PhoneNumber to,
        Body body,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[CONSOLE SMS] To: {To} | Body: {Body}",
            to.Value,
            body.Value);

        return Task.CompletedTask;
    }
}
