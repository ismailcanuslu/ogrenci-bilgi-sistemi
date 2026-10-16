using Microsoft.EntityFrameworkCore;
using StudentApplications.Application.Abstractions;
using StudentApplications.Domain.Applications;

namespace StudentApplications.Infrastructure.Persistence.Repositories;

public sealed class StudentApplicationRepository : IStudentApplicationRepository
{
    private readonly StudentApplicationsDbContext _dbContext;

    public StudentApplicationRepository(StudentApplicationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<StudentApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.StudentApplications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudentApplication>> ListAsync(
        Guid? studentId = null,
        ApplicationType? type = null,
        ApplicationState? state = null,
        Guid? targetDepartmentId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<StudentApplication> query = _dbContext.StudentApplications;

        if (studentId.HasValue)
        {
            query = query.Where(x => x.StudentId == studentId.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(x => x.Type == type.Value);
        }

        if (state.HasValue)
        {
            query = query.Where(x => x.State == state.Value);
        }

        if (targetDepartmentId.HasValue)
        {
            query = query.Where(x => x.TargetDepartmentId == targetDepartmentId.Value);
        }

        return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(StudentApplication application, CancellationToken cancellationToken = default)
    {
        await _dbContext.StudentApplications.AddAsync(application, cancellationToken);
    }
}
