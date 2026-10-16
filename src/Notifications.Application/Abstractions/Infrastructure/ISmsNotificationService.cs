using BuildingBlocks.Application.Results;
using Notifications.Application.Features;

namespace Notifications.Application.Abstractions.Infrastructure;

public interface ISmsNotificationService
{
    Task<ServiceResult<SendSmsNotification.Response>> SendOtpSmsAsync(
        string to,
        string otpCode,
        int expiresInMinutes = 15,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<SendSmsNotification.Response>> SendSmsAsync(
        string to,
        string message,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<SendBroadcastSmsNotification.Response>> SendBroadcastSmsAsync(
        IReadOnlyList<string> recipients,
        string message,
        CancellationToken cancellationToken = default);
}
