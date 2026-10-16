using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using SupportDesk.Application.Abstractions;
using SupportDesk.Domain.Tickets;

namespace SupportDesk.Application.Features.Tickets;

public sealed record SupportTicketSummaryResponse(
    Guid Id,
    string TicketNumber,
    Guid RequesterUserId,
    SupportUnit TargetUnit,
    string Subject,
    TicketPriority Priority,
    TicketStatus Status,
    Guid? AssignedStaffUserId,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    int MessageCount);

public sealed record ListSupportTicketsQuery(
    SupportUnit? TargetUnit,
    TicketStatus? Status,
    bool OnlyMyTickets = false) : IRequest<ServiceResult<IReadOnlyCollection<SupportTicketSummaryResponse>>>;

public sealed class ListSupportTicketsHandler(
    ISupportTicketRepository ticketRepository,
    ICurrentUser currentUser,
    IPermissionGuard permissionGuard) : IRequestHandler<ListSupportTicketsQuery, ServiceResult<IReadOnlyCollection<SupportTicketSummaryResponse>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<SupportTicketSummaryResponse>>> Handle(
        ListSupportTicketsQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<IReadOnlyCollection<SupportTicketSummaryResponse>>.Unauthorized(
                new Error(PermissionGuard.UnauthenticatedCode, ErrorType.Unauthorized));
        }

        IReadOnlyCollection<SupportTicket> tickets;

        if (request.OnlyMyTickets)
        {
            tickets = await ticketRepository.ListAsync(userId, request.TargetUnit, request.Status, cancellationToken);
        }
        else
        {
            var staffAuth = await permissionGuard.CheckAsync(Permissions.SupportDesk.TicketManage, [ResourceScope.University], cancellationToken);
            if (staffAuth is not null)
            {
                // Personel yetkisi yoksa sadece kendi biletlerini görebilir
                tickets = await ticketRepository.ListAsync(userId, request.TargetUnit, request.Status, cancellationToken);
            }
            else
            {
                tickets = await ticketRepository.ListAsync(null, request.TargetUnit, request.Status, cancellationToken);
            }
        }

        var dtos = tickets.Select(t => new SupportTicketSummaryResponse(
            t.Id,
            t.TicketNumber,
            t.RequesterUserId,
            t.TargetUnit,
            t.Subject,
            t.Priority,
            t.Status,
            t.AssignedStaffUserId,
            t.CreatedAtUtc,
            t.UpdatedAtUtc,
            t.Messages.Count)).ToList();

        return ServiceResult<IReadOnlyCollection<SupportTicketSummaryResponse>>.Ok(dtos);
    }
}
