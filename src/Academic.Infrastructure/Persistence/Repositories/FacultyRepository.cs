using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Faculties;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class FacultyRepository(AcademicDbContext dbContext) : IFacultyRepository
{
    public Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Faculties.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> CodeExistsAsync(string code, Guid? excludeId, CancellationToken cancellationToken = default)
        => dbContext.Faculties.AnyAsync(
            x => x.Code == code && (excludeId == null || x.Id != excludeId),
            cancellationToken);

    public async Task AddAsync(Faculty faculty, CancellationToken cancellationToken = default)
        => await dbContext.Faculties.AddAsync(faculty, cancellationToken);

    public async Task<PagedResult<Faculty>> ListAsync(
        FacultyListCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Faculties.AsNoTracking().AsQueryable();

        if (criteria.AllowedFacultyIds is not null)
        {
            var allowed = criteria.AllowedFacultyIds;
            query = query.Where(x => allowed.Contains(x.Id));
        }

        if (criteria.IsActive is { } isActive)
        {
            query = query.Where(x => x.IsActive == isActive);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var term = criteria.Search;
            query = query.Where(x => x.Name.Contains(term) || x.Code.Contains(term));
        }

        var desc = criteria.SortDirection == SortDirection.Desc;

        var ordered = criteria.SortBy.ToLowerInvariant() switch
        {
            "code" => desc ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
            "createdat" => desc ? query.OrderByDescending(x => x.CreatedAtUtc) : query.OrderBy(x => x.CreatedAtUtc),
            _ => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name)
        };

        var total = await ordered.CountAsync(cancellationToken);

        var items = await ordered
            .ThenBy(x => x.Id)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Faculty>(items, criteria.Page, criteria.PageSize, total);
    }
}
