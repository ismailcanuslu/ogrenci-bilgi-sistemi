using BuildingBlocks.Application.Results;
using MediatR;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Application.Abstractions.Templates;
using Notifications.Application.Features;
using Notifications.Application.Models;
using Notifications.Domain.Notifications;

namespace Notifications.Infrastructure.Email;

public sealed class EmailNotificationService(
    ISender mediator,
    ITemplateRenderer templateRenderer)
    : IEmailNotificationService
{
    public Task<ServiceResult<SendEmailNotification.Response>> SendOtpEmailAsync(
        string to,
        string otpCode,
        int expiresInMinutes = 5,
        string? heading = null,
        CancellationToken cancellationToken = default)
    {
        var request = new TransactionalEmailRequest(
            To: to,
            Subject: $"Doğrulama Kodunuz: {otpCode}",
            Heading: heading ?? "Doğrulama Kodunuz:",
            Message: "Giriş işleminize devam etmek için lütfen aşağıdaki tek kullanımlık doğrulama kodunu kullanın.",
            Preheader: $"Doğrulama kodunuz: {otpCode}",
            OtpCode: otpCode,
            ExpiresInMinutes: expiresInMinutes,
            Disclaimer: "Bu işlemi siz talep etmediyseniz lütfen bu e-postayı yok sayın. Güvenliğiniz için bu kodu kimseyle paylaşmayın.");

        return SendTransactionalEmailAsync(request, cancellationToken);
    }

    public Task<ServiceResult<SendEmailNotification.Response>> SendActionEmailAsync(
        string to,
        string subject,
        string heading,
        string message,
        string buttonText,
        string buttonUrl,
        int? expiresInMinutes = null,
        CancellationToken cancellationToken = default)
    {
        var request = new TransactionalEmailRequest(
            To: to,
            Subject: subject,
            Heading: heading,
            Message: message,
            Preheader: subject,
            ButtonText: buttonText,
            ButtonUrl: buttonUrl,
            ExpiresInMinutes: expiresInMinutes);

        return SendTransactionalEmailAsync(request, cancellationToken);
    }

    public async Task<ServiceResult<SendEmailNotification.Response>> SendTransactionalEmailAsync(
        TransactionalEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Subject"] = request.Subject,
            ["Heading"] = request.Heading,
            ["Message"] = request.Message ?? string.Empty
        };

        if (!string.IsNullOrWhiteSpace(request.Preheader))
        {
            parameters["Preheader"] = request.Preheader;
        }

        if (!string.IsNullOrWhiteSpace(request.OtpCode))
        {
            parameters["OtpCode"] = request.OtpCode;
        }

        if (!string.IsNullOrWhiteSpace(request.ButtonUrl))
        {
            parameters["ButtonUrl"] = request.ButtonUrl;
            parameters["ButtonText"] = request.ButtonText ?? "Devam Et";
        }

        if (request.ExpiresInMinutes.HasValue)
        {
            parameters["ExpiresInMinutes"] = request.ExpiresInMinutes.Value.ToString();
        }

        if (!string.IsNullOrWhiteSpace(request.Disclaimer))
        {
            parameters["Disclaimer"] = request.Disclaimer;
        }

        if (!string.IsNullOrWhiteSpace(request.Theme))
        {
            parameters["Theme"] = request.Theme;
        }

        var renderedHtml = await templateRenderer.RenderAsync(
            NotificationTemplates.Common.Transactional,
            parameters,
            cancellationToken);

        var command = new SendEmailNotification.Command(
            To: request.To,
            Subject: request.Subject,
            Body: renderedHtml,
            IsHtml: true);

        return await mediator.Send(command, cancellationToken);
    }
}
