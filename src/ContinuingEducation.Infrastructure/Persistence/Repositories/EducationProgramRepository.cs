using ContinuingEducation.Application.Abstractions;
using ContinuingEducation.Domain.Programs;
using Microsoft.EntityFrameworkCore;

namespace ContinuingEducation.Infrastructure.Persistence.Repositories;

public sealed class EducationProgramRepository : IEducationProgramRepository
{
    private readonly ContinuingEducationDbContext _dbContext;

    public EducationProgramRepository(ContinuingEducationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<EducationProgram?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.EducationPrograms.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<EducationProgram?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return _dbContext.EducationPrograms.FirstOrDefaultAsync(x => x.Code == normalized, cancellationToken);
    }

    public async Task<IReadOnlyCollection<EducationProgram>> ListAsync(
        CourseProgramStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EducationProgram> query = _dbContext.EducationPrograms;

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(EducationProgram program, CancellationToken cancellationToken = default)
    {
        await _dbContext.EducationPrograms.AddAsync(program, cancellationToken);
    }
}
