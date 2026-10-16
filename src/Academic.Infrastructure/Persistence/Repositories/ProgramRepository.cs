using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Programs;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class ProgramRepository(AcademicDbContext dbContext) : IProgramRepository
{
    public Task<Program?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Programs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> CodeExistsInDepartmentAsync(
        Guid departmentId,
        string code,
        Guid? excludeId,
        CancellationToken cancellationToken = default)
        => dbContext.Programs.AnyAsync(
            x => x.DepartmentId == departmentId && x.Code == code && (excludeId == null || x.Id != excludeId),
            cancellationToken);

    public async Task AddAsync(Program program, CancellationToken cancellationToken = default)
        => await dbContext.Programs.AddAsync(program, cancellationToken);

    public async Task<PagedResult<Program>> ListAsync(
        ProgramListCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Programs.AsNoTracking().AsQueryable();

        if (!criteria.Unrestricted)
        {
            var allowedFacultyIds = criteria.AllowedFacultyIds;
            var allowedDepartmentIds = criteria.AllowedDepartmentIds;

            // Join with Departments to check department or faculty authorization
            query = from p in query
                    join d in dbContext.Departments on p.DepartmentId equals d.Id
                    where allowedFacultyIds.Contains(d.FacultyId) || allowedDepartmentIds.Contains(d.Id)
                    select p;
        }

        if (criteria.DepartmentId is { } departmentId)
        {
            query = query.Where(x => x.DepartmentId == departmentId);
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

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Code)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Program>(items, criteria.Page, criteria.PageSize, total);
    }
}
