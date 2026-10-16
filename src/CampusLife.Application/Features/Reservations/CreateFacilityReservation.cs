using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using CampusLife.Application.Abstractions;
using CampusLife.Domain.Reservations;
using FluentValidation;
using MediatR;

namespace CampusLife.Application.Features.Reservations;

public sealed record CreateFacilityReservationCommand(
    FacilityType FacilityType,
    string FacilityName,
    DateOnly ReservationDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string? Notes) : IRequest<ServiceResult<Guid>>;

public sealed class CreateFacilityReservationValidator : AbstractValidator<CreateFacilityReservationCommand>
{
    public CreateFacilityReservationValidator()
    {
        RuleFor(x => x.FacilityType).IsInEnum().WithMessage("FacilityReservation.InvalidType");
        RuleFor(x => x.FacilityName)
            .NotEmpty().WithMessage("FacilityReservation.FacilityNameRequired")
            .MaximumLength(FacilityReservation.FacilityNameMaxLength);
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime).WithMessage("FacilityReservation.InvalidTimeRange");
        RuleFor(x => x.Notes).MaximumLength(FacilityReservation.NotesMaxLength);
    }
}

public sealed class CreateFacilityReservationHandler(
    IFacilityReservationRepository reservationRepository,
    ICampusLifeUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IAuditService auditService) : IRequestHandler<CreateFacilityReservationCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(CreateFacilityReservationCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<Guid>.Unauthorized(new Error(PermissionGuard.UnauthenticatedCode, ErrorType.Unauthorized));
        }

        var conflict = await reservationRepository.HasConflictAsync(
            request.FacilityName,
            request.ReservationDate,
            request.StartTime,
            request.EndTime,
            null,
            cancellationToken);

        if (conflict)
        {
            return ServiceResult<Guid>.Conflict(new Error("FacilityReservation.Conflict", ErrorType.Conflict));
        }

        var reservation = FacilityReservation.Create(
            userId,
            request.FacilityType,
            request.FacilityName,
            request.ReservationDate,
            request.StartTime,
            request.EndTime,
            request.Notes,
            DateTime.UtcNow);

        await reservationRepository.AddAsync(reservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "CampusLife.ReservationCreated",
            "CampusLife",
            nameof(FacilityReservation),
            reservation.Id.ToString(),
            newValues: new
            {
                reservation.Id,
                request.FacilityType,
                request.FacilityName,
                request.ReservationDate,
                request.StartTime,
                request.EndTime
            },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(reservation.Id);
    }
}
