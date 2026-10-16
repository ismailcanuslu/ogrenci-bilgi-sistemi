using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;
using SupportDesk.Application.Abstractions;
using SupportDesk.Domain.Tickets;

namespace SupportDesk.Application.Features.Tickets;

public sealed record ReplyToSupportTicketCommand(
    Guid TicketId,
    string Message,
    string? AttachmentUrl = null) : IRequest<ServiceResult<Unit>>;

public sealed class ReplyToSupportTicketValidator : AbstractValidator<ReplyToSupportTicketCommand>
{
    public ReplyToSupportTicketValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.Message).NotEmpty().MaximumLength(TicketMessage.ContentMaxLength);
    }
}

public sealed class ReplyToSupportTicketHandler(
    ISupportTicketRepository ticketRepository,
    ISupportDeskUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IPermissionGuard permissionGuard,
    IAuditService auditService) : IRequestHandler<ReplyToSupportTicketCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(ReplyToSupportTicketCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<Unit>.Unauthorized(new Error(PermissionGuard.UnauthenticatedCode, ErrorType.Unauthorized));
        }

        var ticket = await ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
        {
            return ServiceResult<Unit>.NotFound(new Error("SupportTicket.NotFound", ErrorType.NotFound));
        }

        // Görevli yanıtı mı yoksa talep sahibi yanıtı mı kontrol edilir
        var staffAuth = await permissionGuard.CheckAsync(Permissions.SupportDesk.TicketManage, [ResourceScope.University], cancellationToken);
        var isStaff = staffAuth is null;

        if (!isStaff && ticket.RequesterUserId != userId)
        {
            return ServiceResult<Unit>.Forbidden(new Error(PermissionGuard.ForbiddenCode, ErrorType.Forbidden));
        }

        ticket.AddMessage(userId, isStaff, request.Message, request.AttachmentUrl, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "SupportDesk.TicketReplied",
            "SupportDesk",
            nameof(SupportTicket),
            ticket.Id.ToString(),
            newValues: new { ticket.Id, SenderUserId = userId, isStaff },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
