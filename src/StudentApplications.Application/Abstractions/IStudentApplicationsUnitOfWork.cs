namespace StudentApplications.Application.Abstractions;

public interface IStudentApplicationsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
