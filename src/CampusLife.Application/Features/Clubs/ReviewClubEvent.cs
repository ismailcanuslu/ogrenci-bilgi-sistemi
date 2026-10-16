using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using CampusLife.Application.Abstractions;
using CampusLife.Domain.Clubs;
using FluentValidation;
using MediatR;

namespace CampusLife.Application.Features.Clubs;

public sealed record ReviewClubEventCommand(
    Guid EventId,
    bool IsApproved,
    string? Notes = null) : IRequest<ServiceResult<Unit>>;

public sealed class ReviewClubEventValidator : AbstractValidator<ReviewClubEventCommand>
{
    public ReviewClubEventValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(ClubEvent.ReviewNotesMaxLength);
    }
}

public sealed class ReviewClubEventHandler(
    IStudentClubRepository clubRepository,
    ICampusLifeUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IPermissionGuard permissionGuard,
    IAuditService auditService) : IRequestHandler<ReviewClubEventCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(ReviewClubEventCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(Permissions.CampusLife.EventManage, [ResourceScope.University], cancellationToken);
        if (authError is not null)
        {
            return AuthorizationFailure.To<Unit>(authError);
        }

        if (currentUser.UserId is not { } reviewerId || reviewerId == Guid.Empty)
        {
            return ServiceResult<Unit>.Unauthorized(new Error(PermissionGuard.UnauthenticatedCode, ErrorType.Unauthorized));
        }

        var clubEvent = await clubRepository.GetEventByIdAsync(request.EventId, cancellationToken);
        if (clubEvent is null)
        {
            return ServiceResult<Unit>.NotFound(new Error("ClubEvent.NotFound", ErrorType.NotFound));
        }

        if (request.IsApproved)
        {
            clubEvent.Approve(reviewerId, request.Notes, DateTime.UtcNow);
        }
        else
        {
            clubEvent.Reject(reviewerId, request.Notes ?? "Etkinlik başvurusu reddedildi", DateTime.UtcNow);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            request.IsApproved ? "CampusLife.EventApproved" : "CampusLife.EventRejected",
            "CampusLife",
            nameof(ClubEvent),
            clubEvent.Id.ToString(),
            newValues: new { request.EventId, request.IsApproved, request.Notes, ReviewerId = reviewerId },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
