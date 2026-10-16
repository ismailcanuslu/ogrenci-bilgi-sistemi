using Academic.Application.Common;
using Academic.Domain.Programs;

namespace Academic.Application.Abstractions;

public sealed record ProgramListCriteria(
    bool Unrestricted,
    IReadOnlyCollection<Guid> AllowedFacultyIds,
    IReadOnlyCollection<Guid> AllowedDepartmentIds,
    Guid? DepartmentId,
    string? Search,
    bool? IsActive,
    int Page,
    int PageSize);

public interface IProgramRepository
{
    Task<Program?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsInDepartmentAsync(
        Guid departmentId,
        string code,
        Guid? excludeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Program program, CancellationToken cancellationToken = default);

    Task<PagedResult<Program>> ListAsync(
        ProgramListCriteria criteria,
        CancellationToken cancellationToken = default);
}
