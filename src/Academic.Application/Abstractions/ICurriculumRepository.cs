using Academic.Application.Common;
using Academic.Domain.Curriculums;

namespace Academic.Application.Abstractions;

public sealed record CurriculumListCriteria(
    bool Unrestricted,
    IReadOnlyCollection<Guid> AllowedFacultyIds,
    IReadOnlyCollection<Guid> AllowedDepartmentIds,
    Guid? ProgramId,
    Guid? AcademicYearId,
    string? Search,
    bool? IsActive,
    int Page,
    int PageSize);

public interface ICurriculumRepository
{
    Task<Curriculum?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Curriculum?> GetByVersionIdAsync(Guid versionId, CancellationToken cancellationToken = default);

    Task<bool> ExistsForProgramAndYearAsync(
        Guid programId,
        Guid academicYearId,
        Guid? excludeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Curriculum curriculum, CancellationToken cancellationToken = default);

    Task<PagedResult<Curriculum>> ListAsync(
        CurriculumListCriteria criteria,
        CancellationToken cancellationToken = default);
}
