using Microsoft.Extensions.Logging;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Domain.ValueObjects;

namespace Notifications.Infrastructure.Email;

public sealed class ConsoleEmailSender(ILogger<ConsoleEmailSender> logger) : IEmailSender
{
    public Task SendEmailAsync(
        EmailAddress to,
        Subject subject,
        Body body,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[CONSOLE EMAIL] To: {To} | Subject: {Subject} | Body: {Body}",
            to.Value,
            subject.Value,
            body.Value);

        return Task.CompletedTask;
    }
}
