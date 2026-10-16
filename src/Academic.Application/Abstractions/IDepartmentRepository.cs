using Academic.Application.Common;
using Academic.Domain.Departments;

namespace Academic.Application.Abstractions;

public sealed record DepartmentListCriteria(
    bool Unrestricted,
    IReadOnlyCollection<Guid> AllowedFacultyIds,
    IReadOnlyCollection<Guid> AllowedDepartmentIds,
    Guid? FacultyId,
    string? Search,
    bool? IsActive,
    string SortBy,
    SortDirection SortDirection,
    int Page,
    int PageSize);

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsInFacultyAsync(
        Guid facultyId,
        string code,
        Guid? excludeId,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveDepartmentsAsync(Guid facultyId, CancellationToken cancellationToken = default);

    Task AddAsync(Department department, CancellationToken cancellationToken = default);

    Task<PagedResult<Department>> ListAsync(
        DepartmentListCriteria criteria,
        CancellationToken cancellationToken = default);
}
