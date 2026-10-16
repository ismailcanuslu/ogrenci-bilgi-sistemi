using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SupportDesk.Application.Features.Tickets;
using SupportDesk.Domain.Tickets;

namespace SupportDesk.Api.Endpoints;

public sealed record CreateTicketRequest(
    SupportUnit TargetUnit,
    string Subject,
    TicketPriority Priority,
    string Message,
    string? AttachmentUrl);

public sealed record ReplyTicketRequest(
    string Message,
    string? AttachmentUrl);

public sealed record ResolveTicketRequest(string? ResolutionMessage);

public static class SupportDeskEndpoints
{
    public static IEndpointRouteBuilder MapSupportDeskEndpoints(this IEndpointRouteBuilder app)
    {
        var tickets = app.MapVersionedGroup("support-desk", 1, 0)
            .MapGroup("/tickets")
            .WithTags("SupportDesk")
            .RequireAuthorization();

        tickets.MapPost("/", async (
            CreateTicketRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateSupportTicketCommand(
                body.TargetUnit,
                body.Subject,
                body.Priority,
                body.Message,
                body.AttachmentUrl), ct)).ToHttpResult(http))
            .WithName("CreateSupportTicket")
            .WithSummary("Kurum/birim seçerek servis destek talebi oluşturur");

        tickets.MapGet("/", async (
            [AsParameters] ListSupportTicketsQuery query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListSupportTickets")
            .WithSummary("Destek taleplerini listeler (kendi talepleri veya yetkili birim filtreli)");

        tickets.MapGet("/{id:guid}", async (
            Guid id, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetSupportTicketQuery(id), ct)).ToHttpResult(http))
            .WithName("GetSupportTicket")
            .WithSummary("Destek talebi ve mesajlaşma geçmişi detayı");

        tickets.MapPost("/{id:guid}/reply", async (
            Guid id, ReplyTicketRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ReplyToSupportTicketCommand(id, body.Message, body.AttachmentUrl), ct)).ToHttpResult(http))
            .WithName("ReplyToSupportTicket")
            .WithSummary("Destek talebine yanıt / mesaj ekler");

        tickets.MapPut("/{id:guid}/resolve", async (
            Guid id, ResolveTicketRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ResolveSupportTicketCommand(id, body.ResolutionMessage), ct)).ToHttpResult(http))
            .WithName("ResolveSupportTicket")
            .WithSummary("Destek talebini çözümlendi olarak işaretler (support-desk.ticket.manage)");

        return app;
    }
}
