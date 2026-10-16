using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class CourseRepository(AcademicDbContext dbContext) : ICourseRepository
{
    public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Courses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => dbContext.Courses.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);

    public Task<bool> CodeExistsAsync(string code, Guid? excludeId, CancellationToken cancellationToken = default)
        => dbContext.Courses.AnyAsync(x => x.Code == code && (excludeId == null || x.Id != excludeId), cancellationToken);

    public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
        => await dbContext.Courses.AddAsync(course, cancellationToken);

    public async Task<PagedResult<Course>> ListAsync(CourseListCriteria criteria, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Courses.AsNoTracking().AsQueryable();

        if (criteria.DepartmentIds is { Count: > 0 } deptIds)
        {
            query = query.Where(x => deptIds.Contains(x.DepartmentId));
        }

        if (criteria.IsActive is { } isActive)
        {
            query = query.Where(x => x.IsActive == isActive);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var term = criteria.Search.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Code.ToLower().Contains(term) ||
                x.Name.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Code)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Course>(items, criteria.Page, criteria.PageSize, total);
    }
}
