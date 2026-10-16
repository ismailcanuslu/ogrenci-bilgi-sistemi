using SocialContribution.Domain;

namespace SocialContribution.Application.Abstractions;

public interface ISocialContributionRepository
{
    Task<SocialContributionProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SocialContributionProject>> GetAllAsync(
        ContributionCategory? category = null,
        ProjectStatus? status = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SocialContributionProject>> GetByCoordinatorAsync(Guid coordinatorUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SocialContributionProject>> GetUserVolunteeredProjectsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(SocialContributionProject project, CancellationToken cancellationToken = default);
    Task UpdateAsync(SocialContributionProject project, CancellationToken cancellationToken = default);
}
