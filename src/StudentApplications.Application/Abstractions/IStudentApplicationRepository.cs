using StudentApplications.Domain.Applications;

namespace StudentApplications.Application.Abstractions;

public interface IStudentApplicationRepository
{
    Task<StudentApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<StudentApplication>> ListAsync(
        Guid? studentId = null,
        ApplicationType? type = null,
        ApplicationState? state = null,
        Guid? targetDepartmentId = null,
        CancellationToken cancellationToken = default);
    Task AddAsync(StudentApplication application, CancellationToken cancellationToken = default);
}
