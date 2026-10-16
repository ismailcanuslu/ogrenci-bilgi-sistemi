using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Application.Features;

namespace Notifications.Api.Endpoints;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationsEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("notifications", 1, 0)
            .WithTags("Notifications");

        group.MapPost("/email", async (
            SendEmailNotification.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("SendEmailNotification")
        .WithSummary("E-posta bildirimi gönderme");

        group.MapPost("/email/templated", async (
            SendTemplatedEmailNotification.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("SendTemplatedEmailNotification")
        .WithSummary("Şablonlu e-posta bildirimi gönderme");

        group.MapPost("/sms", async (
            SendSmsNotification.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("SendSmsNotification")
        .WithSummary("Serbest SMS bildirimi gönderme (Tekil, serbest metin/bakım/duyuru)");

        group.MapPost("/sms/broadcast", async (
            SendBroadcastSmsNotification.Command command,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("SendBroadcastSmsNotification")
        .WithSummary("Toplu serbest SMS bildirimi gönderme (Çoklu alıcı, sistem bakımı/acil duyuru)");

        group.MapPost("/sms/otp", async (
            SendOtpSmsRequest request,
            ISmsNotificationService smsService,
            CancellationToken cancellationToken) =>
        {
            var result = await smsService.SendOtpSmsAsync(
                request.To,
                request.OtpCode,
                request.ExpiresInMinutes,
                cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("SendOtpSmsNotification")
        .WithSummary("Doğrulama OTP SMS bildirimi gönderme");

        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetNotificationStatus.Query(id);
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("GetNotificationStatus")
        .WithSummary("Bildirim durumunu sorgulama");

        group.MapPost("/{id:guid}/retry", async (
            Guid id,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new RetryNotification.Command(id);
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("RetryNotification")
        .WithSummary("Başarısız bildirimi yeniden gönderme");

        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new CancelNotification.Command(id);
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("CancelNotification")
        .WithSummary("Bekleyen bildirimi iptal etme");

        group.MapGet("/", async (
            int page,
            int pageSize,
            string? recipient,
            string? status,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetNotificationsHistory.Query(
                page <= 0 ? 1 : page,
                pageSize <= 0 ? 20 : pageSize,
                recipient,
                status);
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("GetNotificationsHistory")
        .WithSummary("Bildirim geçmişini sayfalı olarak listeleme");

        return app;
    }

    public sealed record SendOtpSmsRequest(
        string To,
        string OtpCode,
        int ExpiresInMinutes = 15);
}
