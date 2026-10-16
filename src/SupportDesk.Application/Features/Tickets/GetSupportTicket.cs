using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using SupportDesk.Application.Abstractions;
using SupportDesk.Domain.Tickets;

namespace SupportDesk.Application.Features.Tickets;

public sealed record TicketMessageResponse(
    Guid Id,
    Guid SenderUserId,
    bool IsFromStaff,
    string Content,
    string? AttachmentUrl,
    DateTime SentAtUtc);

public sealed record SupportTicketResponse(
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
    DateTime? ClosedAtUtc,
    IReadOnlyCollection<TicketMessageResponse> Messages);

public sealed record GetSupportTicketQuery(Guid Id) : IRequest<ServiceResult<SupportTicketResponse>>;

public sealed class GetSupportTicketHandler(
    ISupportTicketRepository ticketRepository,
    ICurrentUser currentUser,
    IPermissionGuard permissionGuard) : IRequestHandler<GetSupportTicketQuery, ServiceResult<SupportTicketResponse>>
{
    public async Task<ServiceResult<SupportTicketResponse>> Handle(GetSupportTicketQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<SupportTicketResponse>.Unauthorized(
                new Error(PermissionGuard.UnauthenticatedCode, ErrorType.Unauthorized));
        }

        var ticket = await ticketRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ticket is null)
        {
            return ServiceResult<SupportTicketResponse>.NotFound(new Error("SupportTicket.NotFound", ErrorType.NotFound));
        }

        var staffAuth = await permissionGuard.CheckAsync(Permissions.SupportDesk.TicketManage, [ResourceScope.University], cancellationToken);
        if (staffAuth is not null && ticket.RequesterUserId != userId)
        {
            return ServiceResult<SupportTicketResponse>.Forbidden(new Error(PermissionGuard.ForbiddenCode, ErrorType.Forbidden));
        }

        var messages = ticket.Messages.Select(m => new TicketMessageResponse(
            m.Id,
            m.SenderUserId,
            m.IsFromStaff,
            m.Content,
            m.AttachmentUrl,
            m.SentAtUtc)).ToList();

        return ServiceResult<SupportTicketResponse>.Ok(new SupportTicketResponse(
            ticket.Id,
            ticket.TicketNumber,
            ticket.RequesterUserId,
            ticket.TargetUnit,
            ticket.Subject,
            ticket.Priority,
            ticket.Status,
            ticket.AssignedStaffUserId,
            ticket.CreatedAtUtc,
            ticket.UpdatedAtUtc,
            ticket.ClosedAtUtc,
            messages));
    }
}
