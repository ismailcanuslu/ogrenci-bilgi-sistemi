using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using CampusLife.Application.Abstractions;
using CampusLife.Domain.Reservations;
using FluentValidation;
using MediatR;

namespace CampusLife.Application.Features.Reservations;

public sealed record ReviewFacilityReservationCommand(
    Guid ReservationId,
    bool IsApproved,
    string? Notes = null) : IRequest<ServiceResult<Unit>>;

public sealed class ReviewFacilityReservationValidator : AbstractValidator<ReviewFacilityReservationCommand>
{
    public ReviewFacilityReservationValidator()
    {
        RuleFor(x => x.ReservationId).NotEmpty().WithMessage("FacilityReservation.IdRequired");
        RuleFor(x => x.Notes).MaximumLength(FacilityReservation.ReviewNotesMaxLength);
    }
}

public sealed class ReviewFacilityReservationHandler(
    IFacilityReservationRepository reservationRepository,
    ICampusLifeUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IPermissionGuard permissionGuard,
    IAuditService auditService) : IRequestHandler<ReviewFacilityReservationCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(ReviewFacilityReservationCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(Permissions.CampusLife.ReservationManage, [ResourceScope.University], cancellationToken);
        if (authError is not null)
        {
            return AuthorizationFailure.To<Unit>(authError);
        }

        if (currentUser.UserId is not { } reviewerId || reviewerId == Guid.Empty)
        {
            return ServiceResult<Unit>.Unauthorized(new Error(PermissionGuard.UnauthenticatedCode, ErrorType.Unauthorized));
        }

        var reservation = await reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
        {
            return ServiceResult<Unit>.NotFound(new Error("FacilityReservation.NotFound", ErrorType.NotFound));
        }

        if (request.IsApproved)
        {
            reservation.Approve(reviewerId, request.Notes, DateTime.UtcNow);
        }
        else
        {
            reservation.Reject(reviewerId, request.Notes ?? "Yönetici tarafından reddedildi", DateTime.UtcNow);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            request.IsApproved ? "CampusLife.ReservationApproved" : "CampusLife.ReservationRejected",
            "CampusLife",
            nameof(FacilityReservation),
            reservation.Id.ToString(),
            newValues: new { request.ReservationId, request.IsApproved, request.Notes, ReviewerId = reviewerId },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
