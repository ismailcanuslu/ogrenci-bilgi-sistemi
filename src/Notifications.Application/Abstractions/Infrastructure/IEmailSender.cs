using Notifications.Domain.ValueObjects;

namespace Notifications.Application.Abstractions.Infrastructure;

public interface IEmailSender
{
    Task SendEmailAsync(
        EmailAddress to,
        Subject subject,
        Body body,
        CancellationToken cancellationToken = default);
}
