using CampusLife.Application.Abstractions;
using CampusLife.Domain.Clubs;
using Microsoft.EntityFrameworkCore;

namespace CampusLife.Infrastructure.Persistence.Repositories;

public sealed class StudentClubRepository(CampusLifeDbContext dbContext) : IStudentClubRepository
{
    public Task<StudentClub?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.StudentClubs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<StudentClub?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => dbContext.StudentClubs.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<StudentClub>> ListAsync(bool? activeOnly = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.StudentClubs.AsQueryable();
        if (activeOnly.HasValue)
        {
            query = query.Where(x => x.IsActive == activeOnly.Value);
        }

        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task AddClubAsync(StudentClub club, CancellationToken cancellationToken = default)
        => await dbContext.StudentClubs.AddAsync(club, cancellationToken);

    public Task<ClubEvent?> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        => dbContext.ClubEvents.FirstOrDefaultAsync(x => x.Id == eventId, cancellationToken);

    public async Task<IReadOnlyCollection<ClubEvent>> ListEventsAsync(
        Guid? clubId = null, EventStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.ClubEvents.AsQueryable();
        if (clubId.HasValue)
        {
            query = query.Where(x => x.ClubId == clubId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query.OrderByDescending(x => x.EventDateUtc).ToListAsync(cancellationToken);
    }

    public async Task AddEventAsync(ClubEvent clubEvent, CancellationToken cancellationToken = default)
        => await dbContext.ClubEvents.AddAsync(clubEvent, cancellationToken);
}
