namespace ContinuingEducation.Application.Abstractions;

public interface IContinuingEducationUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
