using ContinuingEducation.Application.Abstractions;
using ContinuingEducation.Domain.Programs;
using Microsoft.EntityFrameworkCore;

namespace ContinuingEducation.Infrastructure.Persistence.Repositories;

public sealed class ProgramApplicationRepository : IProgramApplicationRepository
{
    private readonly ContinuingEducationDbContext _dbContext;

    public ProgramApplicationRepository(ContinuingEducationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ProgramApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.ProgramApplications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<ProgramApplication?> GetByProgramAndApplicantAsync(
        Guid programId,
        Guid applicantUserId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ProgramApplications.FirstOrDefaultAsync(
            x => x.ProgramId == programId && x.ApplicantUserId == applicantUserId,
            cancellationToken);
    }

    public Task<ProgramApplication?> GetByUserAndProgramAsync(
        Guid userId,
        Guid programId,
        CancellationToken cancellationToken = default)
    {
        return GetByProgramAndApplicantAsync(programId, userId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProgramApplication>> ListAsync(
        Guid? programId = null,
        Guid? applicantUserId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ProgramApplication> query = _dbContext.ProgramApplications;

        if (programId.HasValue)
        {
            query = query.Where(x => x.ProgramId == programId.Value);
        }

        if (applicantUserId.HasValue)
        {
            query = query.Where(x => x.ApplicantUserId == applicantUserId.Value);
        }

        return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<ProgramApplication>> ListByProgramIdAsync(
        Guid programId,
        CancellationToken cancellationToken = default)
    {
        return ListAsync(programId, null, cancellationToken);
    }

    public Task<IReadOnlyCollection<ProgramApplication>> ListByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return ListAsync(null, userId, cancellationToken);
    }

    public async Task AddAsync(ProgramApplication application, CancellationToken cancellationToken = default)
    {
        await _dbContext.ProgramApplications.AddAsync(application, cancellationToken);
    }
}
