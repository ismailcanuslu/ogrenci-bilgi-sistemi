using BuildingBlocks.Application.Results;
using MediatR;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Application.Features;

namespace Notifications.Infrastructure.Sms;

public sealed class SmsNotificationService(ISender mediator) : ISmsNotificationService
{
    public Task<ServiceResult<SendSmsNotification.Response>> SendOtpSmsAsync(
        string to,
        string otpCode,
        int expiresInMinutes = 15,
        CancellationToken cancellationToken = default)
    {
        var message = $"Doğrulama kodu: {otpCode}. Kod {expiresInMinutes} dakika boyunca geçerlidir.";
        return SendSmsAsync(to, message, cancellationToken);
    }

    public async Task<ServiceResult<SendSmsNotification.Response>> SendSmsAsync(
        string to,
        string message,
        CancellationToken cancellationToken = default)
    {
        var command = new SendSmsNotification.Command(to, message);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<ServiceResult<SendBroadcastSmsNotification.Response>> SendBroadcastSmsAsync(
        IReadOnlyList<string> recipients,
        string message,
        CancellationToken cancellationToken = default)
    {
        var command = new SendBroadcastSmsNotification.Command(recipients, message);
        return await mediator.Send(command, cancellationToken);
    }
}
