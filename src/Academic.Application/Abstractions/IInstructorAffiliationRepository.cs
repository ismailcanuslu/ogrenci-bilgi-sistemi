using Academic.Domain.Instructors;

namespace Academic.Application.Abstractions;

public interface IInstructorAffiliationRepository
{
    Task<InstructorDepartmentAffiliation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<InstructorDepartmentAffiliation>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken = default);
    Task AddAsync(InstructorDepartmentAffiliation affiliation, CancellationToken cancellationToken = default);
    void Remove(InstructorDepartmentAffiliation affiliation);
}
