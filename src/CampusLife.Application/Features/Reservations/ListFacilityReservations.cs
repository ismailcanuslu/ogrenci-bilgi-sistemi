using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using CampusLife.Application.Abstractions;
using CampusLife.Domain.Reservations;
using MediatR;

namespace CampusLife.Application.Features.Reservations;

public sealed record FacilityReservationResponse(
    Guid Id,
    Guid UserId,
    FacilityType FacilityType,
    string FacilityName,
    DateOnly ReservationDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    ReservationStatus Status,
    string? Notes,
    Guid? ReviewedByUserId,
    string? ReviewNotes,
    DateTime CreatedAtUtc,
    DateTime? ReviewedAtUtc);

public sealed record ListFacilityReservationsQuery(
    FacilityType? FacilityType,
    ReservationStatus? Status,
    DateOnly? Date,
    bool OnlyMyReservations = false) : IRequest<ServiceResult<IReadOnlyCollection<FacilityReservationResponse>>>;

public sealed class ListFacilityReservationsHandler(
    IFacilityReservationRepository reservationRepository,
    ICurrentUser currentUser,
    IPermissionGuard permissionGuard) : IRequestHandler<ListFacilityReservationsQuery, ServiceResult<IReadOnlyCollection<FacilityReservationResponse>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<FacilityReservationResponse>>> Handle(
        ListFacilityReservationsQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<IReadOnlyCollection<FacilityReservationResponse>>.Unauthorized(
                new Error(PermissionGuard.UnauthenticatedCode, ErrorType.Unauthorized));
        }

        IReadOnlyCollection<FacilityReservation> reservations;

        if (request.OnlyMyReservations)
        {
            reservations = await reservationRepository.ListByUserIdAsync(userId, cancellationToken);
        }
        else
        {
            // Tüm rezervasyonları listelemek için yönetici yetkisi denetlenir
            var authError = await permissionGuard.CheckAsync(Permissions.CampusLife.ReservationManage, [ResourceScope.University], cancellationToken);
            if (authError is not null)
            {
                // Yetkisi yoksa yalnızca kendi rezervasyonlarını döner
                reservations = await reservationRepository.ListByUserIdAsync(userId, cancellationToken);
            }
            else
            {
                reservations = await reservationRepository.ListAsync(request.FacilityType, request.Status, request.Date, cancellationToken);
            }
        }

        var dtos = reservations.Select(r => new FacilityReservationResponse(
            r.Id,
            r.UserId,
            r.FacilityType,
            r.FacilityName,
            r.ReservationDate,
            r.StartTime,
            r.EndTime,
            r.Status,
            r.Notes,
            r.ReviewedByUserId,
            r.ReviewNotes,
            r.CreatedAtUtc,
            r.ReviewedAtUtc)).ToList();

        return ServiceResult<IReadOnlyCollection<FacilityReservationResponse>>.Ok(dtos);
    }
}
