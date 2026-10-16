using Academic.Application.Common;
using Academic.Domain.AcademicYears;

namespace Academic.Application.Abstractions;

public sealed record AcademicYearListCriteria(
    string? Search,
    bool? IsActive,
    int Page,
    int PageSize);

public interface IAcademicYearRepository
{
    Task<AcademicYear?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AcademicYear?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(string code, Guid? excludeId, CancellationToken cancellationToken = default);

    Task AddAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);

    Task<PagedResult<AcademicYear>> ListAsync(
        AcademicYearListCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<AcademicTerm?> GetTermByIdAsync(Guid termId, CancellationToken cancellationToken = default);
}
