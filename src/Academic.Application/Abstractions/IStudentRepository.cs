using Academic.Domain.Students;
using Academic.Application.Common;
using BuildingBlocks.Application.Results;

namespace Academic.Application.Abstractions;

public sealed record StudentListCriteria(
    IReadOnlyCollection<Guid>? DepartmentIds,
    Guid? ProgramId,
    StudentStatus? Status,
    int? EnrollmentYear,
    string? Search,
    int Page,
    int PageSize);

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Student?> GetByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default);

    Task<Student?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> StudentNumberExistsAsync(string studentNumber, Guid? excludeId, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, Guid? excludeId, CancellationToken cancellationToken = default);

    Task AddAsync(Student student, CancellationToken cancellationToken = default);

    Task<PagedResult<Student>> ListAsync(StudentListCriteria criteria, CancellationToken cancellationToken = default);
}
