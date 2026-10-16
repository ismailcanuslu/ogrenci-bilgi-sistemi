using CampusLife.Application.Abstractions;
using CampusLife.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace CampusLife.Infrastructure.Persistence.Repositories;

public sealed class FacilityReservationRepository(CampusLifeDbContext dbContext) : IFacilityReservationRepository
{
    public Task<FacilityReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.FacilityReservations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> HasConflictAsync(
        string facilityName,
        DateOnly date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return dbContext.FacilityReservations.AnyAsync(r =>
            r.FacilityName == facilityName &&
            r.ReservationDate == date &&
            (excludeId == null || r.Id != excludeId) &&
            r.Status != ReservationStatus.Rejected &&
            r.Status != ReservationStatus.Cancelled &&
            r.StartTime < endTime &&
            startTime < r.EndTime,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<FacilityReservation>> ListByUserIdAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.FacilityReservations
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.ReservationDate)
            .ThenByDescending(x => x.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<FacilityReservation>> ListAsync(
        FacilityType? facilityType,
        ReservationStatus? status,
        DateOnly? date,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.FacilityReservations.AsQueryable();

        if (facilityType.HasValue)
        {
            query = query.Where(x => x.FacilityType == facilityType.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (date.HasValue)
        {
            query = query.Where(x => x.ReservationDate == date.Value);
        }

        return await query
            .OrderByDescending(x => x.ReservationDate)
            .ThenByDescending(x => x.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(FacilityReservation reservation, CancellationToken cancellationToken = default)
        => await dbContext.FacilityReservations.AddAsync(reservation, cancellationToken);
}
