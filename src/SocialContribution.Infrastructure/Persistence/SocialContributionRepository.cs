using Microsoft.EntityFrameworkCore;
using SocialContribution.Application.Abstractions;
using SocialContribution.Domain;

namespace SocialContribution.Infrastructure.Persistence;

public sealed class SocialContributionRepository : ISocialContributionRepository
{
    private readonly SocialContributionDbContext _dbContext;

    public SocialContributionRepository(SocialContributionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SocialContributionProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SocialContributionProjects
            .Include(p => p.Volunteers)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SocialContributionProject>> GetAllAsync(
        ContributionCategory? category = null,
        ProjectStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.SocialContributionProjects
            .Include(p => p.Volunteers)
            .AsQueryable();

        if (category.HasValue)
            query = query.Where(p => p.Category == category.Value);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        return await query.OrderByDescending(p => p.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SocialContributionProject>> GetByCoordinatorAsync(Guid coordinatorUserId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SocialContributionProjects
            .Include(p => p.Volunteers)
            .Where(p => p.CoordinatorUserId == coordinatorUserId)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SocialContributionProject>> GetUserVolunteeredProjectsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SocialContributionProjects
            .Include(p => p.Volunteers)
            .Where(p => p.Volunteers.Any(v => v.UserId == userId))
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SocialContributionProject project, CancellationToken cancellationToken = default)
    {
        await _dbContext.SocialContributionProjects.AddAsync(project, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SocialContributionProject project, CancellationToken cancellationToken = default)
    {
        _dbContext.SocialContributionProjects.Update(project);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
