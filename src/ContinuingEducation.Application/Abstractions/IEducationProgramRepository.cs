using ContinuingEducation.Domain.Programs;

namespace ContinuingEducation.Application.Abstractions;

public interface IEducationProgramRepository
{
    Task<EducationProgram?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EducationProgram?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<EducationProgram>> ListAsync(CourseProgramStatus? status, CancellationToken cancellationToken = default);
    Task AddAsync(EducationProgram program, CancellationToken cancellationToken = default);
}
