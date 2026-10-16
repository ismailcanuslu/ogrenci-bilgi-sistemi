using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Domain.ValueObjects;
using Notifications.Infrastructure.Options;

namespace Notifications.Infrastructure.Email;

public sealed class SmtpEmailSender(
    IOptions<NotificationOptions> options,
    ILogger<SmtpEmailSender> logger)
    : IEmailSender
{
    public async Task SendEmailAsync(
        EmailAddress to,
        Subject subject,
        Body body,
        CancellationToken cancellationToken = default)
    {
        var smtpConfig = options.Value.Smtp;

        using var client = new SmtpClient(smtpConfig.Host, smtpConfig.Port)
        {
            EnableSsl = smtpConfig.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(smtpConfig.Username) &&
            !string.IsNullOrWhiteSpace(smtpConfig.Password))
        {
            client.Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password);
        }

        using var message = new MailMessage
        {
            From = new MailAddress(smtpConfig.FromEmail, smtpConfig.FromName),
            Subject = subject.Value,
            Body = body.Value,
            IsBodyHtml = body.IsHtml
        };

        message.To.Add(to.Value);

        logger.LogInformation(
            "Sending email via SMTP ({Host}:{Port}) to {To} with subject '{Subject}'",
            smtpConfig.Host,
            smtpConfig.Port,
            to.Value,
            subject.Value);

        await client.SendMailAsync(message, cancellationToken);
    }
}
