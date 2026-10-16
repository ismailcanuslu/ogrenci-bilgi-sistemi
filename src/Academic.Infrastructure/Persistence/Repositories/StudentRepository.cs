using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Students;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class StudentRepository(AcademicDbContext dbContext) : IStudentRepository
{
    public Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Students.Include(x => x.ProgramEnrollments).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Student?> GetByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default)
        => dbContext.Students.Include(x => x.ProgramEnrollments).FirstOrDefaultAsync(x => x.StudentNumber == studentNumber, cancellationToken);

    public Task<Student?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => dbContext.Students.Include(x => x.ProgramEnrollments).FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public Task<bool> StudentNumberExistsAsync(string studentNumber, Guid? excludeId, CancellationToken cancellationToken = default)
        => dbContext.Students.AnyAsync(x => x.StudentNumber == studentNumber && (excludeId == null || x.Id != excludeId), cancellationToken);

    public Task<bool> EmailExistsAsync(string email, Guid? excludeId, CancellationToken cancellationToken = default)
        => dbContext.Students.AnyAsync(x => x.Email == email && (excludeId == null || x.Id != excludeId), cancellationToken);

    public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
        => await dbContext.Students.AddAsync(student, cancellationToken);

    public async Task<PagedResult<Student>> ListAsync(StudentListCriteria criteria, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Students.AsNoTracking().Include(x => x.ProgramEnrollments).AsQueryable();

        if (criteria.DepartmentIds is { Count: > 0 } deptIds)
        {
            query = query.Where(x => deptIds.Contains(x.DepartmentId));
        }

        if (criteria.ProgramId is { } programId)
        {
            query = query.Where(x => x.ProgramId == programId);
        }

        if (criteria.Status is { } status)
        {
            query = query.Where(x => x.Status == status);
        }

        if (criteria.EnrollmentYear is { } year)
        {
            query = query.Where(x => x.EnrollmentYear == year);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var term = criteria.Search.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.StudentNumber.ToLower().Contains(term) ||
                x.FirstName.ToLower().Contains(term) ||
                x.LastName.ToLower().Contains(term) ||
                x.Email.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.EnrollmentDateUtc)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Student>(items, criteria.Page, criteria.PageSize, total);
    }
}
