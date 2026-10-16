using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Curriculums;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class CurriculumRepository(AcademicDbContext dbContext) : ICurriculumRepository
{
    public Task<Curriculum?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Curriculums
            .Include(x => x.Versions)
                .ThenInclude(v => v.Courses)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Curriculum?> GetByVersionIdAsync(Guid versionId, CancellationToken cancellationToken = default)
        => dbContext.Curriculums
            .Include(x => x.Versions)
                .ThenInclude(v => v.Courses)
            .FirstOrDefaultAsync(x => x.Versions.Any(v => v.Id == versionId), cancellationToken);

    public Task<bool> ExistsForProgramAndYearAsync(
        Guid programId,
        Guid academicYearId,
        Guid? excludeId,
        CancellationToken cancellationToken = default)
        => dbContext.Curriculums.AnyAsync(
            x => x.ProgramId == programId && x.AcademicYearId == academicYearId && (excludeId == null || x.Id != excludeId),
            cancellationToken);

    public async Task AddAsync(Curriculum curriculum, CancellationToken cancellationToken = default)
        => await dbContext.Curriculums.AddAsync(curriculum, cancellationToken);

    public async Task<PagedResult<Curriculum>> ListAsync(
        CurriculumListCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Curriculums
            .Include(x => x.Versions)
                .ThenInclude(v => v.Courses)
            .AsNoTracking()
            .AsQueryable();

        if (!criteria.Unrestricted)
        {
            var allowedFacultyIds = criteria.AllowedFacultyIds;
            var allowedDepartmentIds = criteria.AllowedDepartmentIds;

            query = from c in query
                    join p in dbContext.Programs on c.ProgramId equals p.Id
                    join d in dbContext.Departments on p.DepartmentId equals d.Id
                    where allowedFacultyIds.Contains(d.FacultyId) || allowedDepartmentIds.Contains(d.Id)
                    select c;
        }

        if (criteria.ProgramId is { } programId)
        {
            query = query.Where(x => x.ProgramId == programId);
        }

        if (criteria.AcademicYearId is { } yearId)
        {
            query = query.Where(x => x.AcademicYearId == yearId);
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
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Curriculum>(items, criteria.Page, criteria.PageSize, total);
    }
}
