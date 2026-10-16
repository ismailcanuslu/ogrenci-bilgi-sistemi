using CampusLife.Domain.Reservations;

namespace CampusLife.Application.Abstractions;

public interface IFacilityReservationRepository
{
    Task<FacilityReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasConflictAsync(string facilityName, DateOnly date, TimeSpan startTime, TimeSpan endTime, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<FacilityReservation>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<FacilityReservation>> ListAsync(FacilityType? facilityType, ReservationStatus? status, DateOnly? date, CancellationToken cancellationToken = default);
    Task AddAsync(FacilityReservation reservation, CancellationToken cancellationToken = default);
}
