using ContinuingEducation.Domain.Programs;

namespace ContinuingEducation.Application.Abstractions;

public interface IProgramApplicationRepository
{
    Task<ProgramApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProgramApplication?> GetByProgramAndApplicantAsync(Guid programId, Guid applicantUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProgramApplication>> ListAsync(Guid? programId = null, Guid? applicantUserId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProgramApplication>> ListByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProgramApplication>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(ProgramApplication application, CancellationToken cancellationToken = default);
}
