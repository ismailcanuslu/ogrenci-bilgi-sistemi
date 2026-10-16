using CampusLife.Domain.Clubs;

namespace CampusLife.Application.Abstractions;

public interface IStudentClubRepository
{
    Task<StudentClub?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StudentClub?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<StudentClub>> ListAsync(bool? activeOnly = null, CancellationToken cancellationToken = default);
    Task AddClubAsync(StudentClub club, CancellationToken cancellationToken = default);

    Task<ClubEvent?> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ClubEvent>> ListEventsAsync(Guid? clubId = null, EventStatus? status = null, CancellationToken cancellationToken = default);
    Task AddEventAsync(ClubEvent clubEvent, CancellationToken cancellationToken = default);
}
