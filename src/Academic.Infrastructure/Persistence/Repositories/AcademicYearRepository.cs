using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.AcademicYears;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class AcademicYearRepository(AcademicDbContext dbContext) : IAcademicYearRepository
{
    public Task<AcademicYear?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.AcademicYears
            .Include(x => x.Terms)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<AcademicYear?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => dbContext.AcademicYears
            .Include(x => x.Terms)
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);

    public Task<bool> CodeExistsAsync(string code, Guid? excludeId, CancellationToken cancellationToken = default)
        => dbContext.AcademicYears.AnyAsync(
            x => x.Code == code && (excludeId == null || x.Id != excludeId),
            cancellationToken);

    public async Task AddAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
        => await dbContext.AcademicYears.AddAsync(academicYear, cancellationToken);

    public async Task<PagedResult<AcademicYear>> ListAsync(
        AcademicYearListCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.AcademicYears
            .Include(x => x.Terms)
            .AsNoTracking()
            .AsQueryable();

        if (criteria.IsActive is { } isActive)
        {
            query = query.Where(x => x.IsActive == isActive);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var term = criteria.Search;
            query = query.Where(x => x.Name.Contains(term) || x.Code.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.StartDate)
            .ThenBy(x => x.Code)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AcademicYear>(items, criteria.Page, criteria.PageSize, total);
    }

    public Task<AcademicTerm?> GetTermByIdAsync(Guid termId, CancellationToken cancellationToken = default)
        => dbContext.AcademicTerms.FirstOrDefaultAsync(x => x.Id == termId, cancellationToken);
}
