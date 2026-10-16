using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;
using SupportDesk.Application.Abstractions;
using SupportDesk.Domain.Tickets;

namespace SupportDesk.Application.Features.Tickets;

public sealed record ResolveSupportTicketCommand(
    Guid TicketId,
    string? ResolutionMessage = null) : IRequest<ServiceResult<Unit>>;

public sealed class ResolveSupportTicketValidator : AbstractValidator<ResolveSupportTicketCommand>
{
    public ResolveSupportTicketValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
    }
}

public sealed class ResolveSupportTicketHandler(
    ISupportTicketRepository ticketRepository,
    ISupportDeskUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IPermissionGuard permissionGuard,
    IAuditService auditService) : IRequestHandler<ResolveSupportTicketCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(ResolveSupportTicketCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(Permissions.SupportDesk.TicketManage, [ResourceScope.University], cancellationToken);
        if (authError is not null)
        {
            return AuthorizationFailure.To<Unit>(authError);
        }

        if (currentUser.UserId is not { } staffUserId || staffUserId == Guid.Empty)
        {
            return ServiceResult<Unit>.Unauthorized(new Error(PermissionGuard.UnauthenticatedCode, ErrorType.Unauthorized));
        }

        var ticket = await ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
        {
            return ServiceResult<Unit>.NotFound(new Error("SupportTicket.NotFound", ErrorType.NotFound));
        }

        ticket.Resolve(staffUserId, request.ResolutionMessage, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "SupportDesk.TicketResolved",
            "SupportDesk",
            nameof(SupportTicket),
            ticket.Id.ToString(),
            newValues: new { ticket.Id, StaffUserId = staffUserId },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
