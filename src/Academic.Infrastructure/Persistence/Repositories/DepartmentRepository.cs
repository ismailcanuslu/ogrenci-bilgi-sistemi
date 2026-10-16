using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Departments;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class DepartmentRepository(AcademicDbContext dbContext) : IDepartmentRepository
{
    public Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Departments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> CodeExistsInFacultyAsync(
        Guid facultyId,
        string code,
        Guid? excludeId,
        CancellationToken cancellationToken = default)
        => dbContext.Departments.AnyAsync(
            x => x.FacultyId == facultyId && x.Code == code && (excludeId == null || x.Id != excludeId),
            cancellationToken);

    public Task<bool> HasActiveDepartmentsAsync(Guid facultyId, CancellationToken cancellationToken = default)
        => dbContext.Departments.AnyAsync(x => x.FacultyId == facultyId && x.IsActive, cancellationToken);

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
        => await dbContext.Departments.AddAsync(department, cancellationToken);

    public async Task<PagedResult<Department>> ListAsync(
        DepartmentListCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Departments.AsNoTracking().AsQueryable();

        if (!criteria.Unrestricted)
        {
            // Fakülte kapsamı fakültenin tüm bölümlerini, bölüm kapsamı yalnızca atanan bölümü verir.
            var facultyIds = criteria.AllowedFacultyIds;
            var departmentIds = criteria.AllowedDepartmentIds;

            query = query.Where(x => facultyIds.Contains(x.FacultyId) || departmentIds.Contains(x.Id));
        }

        if (criteria.FacultyId is { } facultyId)
        {
            query = query.Where(x => x.FacultyId == facultyId);
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

        return new PagedResult<Department>(items, criteria.Page, criteria.PageSize, total);
    }
}
