using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;
using SupportDesk.Application.Abstractions;
using SupportDesk.Domain.Tickets;

namespace SupportDesk.Application.Features.Tickets;

public sealed record CreateSupportTicketCommand(
    SupportUnit TargetUnit,
    string Subject,
    TicketPriority Priority,
    string Message,
    string? AttachmentUrl = null) : IRequest<ServiceResult<Guid>>;

public sealed class CreateSupportTicketValidator : AbstractValidator<CreateSupportTicketCommand>
{
    public CreateSupportTicketValidator()
    {
        RuleFor(x => x.TargetUnit).IsInEnum().WithMessage("SupportTicket.InvalidTargetUnit");
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(SupportTicket.SubjectMaxLength);
        RuleFor(x => x.Priority).IsInEnum().WithMessage("SupportTicket.InvalidPriority");
        RuleFor(x => x.Message).NotEmpty().MaximumLength(TicketMessage.ContentMaxLength);
    }
}

public sealed class CreateSupportTicketHandler(
    ISupportTicketRepository ticketRepository,
    ISupportDeskUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IAuditService auditService) : IRequestHandler<CreateSupportTicketCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(CreateSupportTicketCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<Guid>.Unauthorized(new Error(PermissionGuard.UnauthenticatedCode, ErrorType.Unauthorized));
        }

        var ticket = SupportTicket.Create(
            userId,
            request.TargetUnit,
            request.Subject,
            request.Priority,
            request.Message,
            request.AttachmentUrl,
            DateTime.UtcNow);

        await ticketRepository.AddAsync(ticket, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "SupportDesk.TicketCreated",
            "SupportDesk",
            nameof(SupportTicket),
            ticket.Id.ToString(),
            newValues: new { ticket.Id, ticket.TicketNumber, request.TargetUnit, request.Subject },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(ticket.Id);
    }
}
