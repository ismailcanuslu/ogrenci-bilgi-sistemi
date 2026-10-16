using BuildingBlocks.Application.Results;
using Notifications.Application.Features;
using Notifications.Application.Models;

namespace Notifications.Application.Abstractions.Infrastructure;

public interface IEmailNotificationService
{
    Task<ServiceResult<SendEmailNotification.Response>> SendOtpEmailAsync(
        string to,
        string otpCode,
        int expiresInMinutes = 5,
        string? heading = null,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<SendEmailNotification.Response>> SendActionEmailAsync(
        string to,
        string subject,
        string heading,
        string message,
        string buttonText,
        string buttonUrl,
        int? expiresInMinutes = null,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<SendEmailNotification.Response>> SendTransactionalEmailAsync(
        TransactionalEmailRequest request,
        CancellationToken cancellationToken = default);
}
