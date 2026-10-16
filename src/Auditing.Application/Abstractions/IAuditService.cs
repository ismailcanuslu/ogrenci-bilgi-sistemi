namespace Auditing.Application.Abstractions;

public interface IAuditService
{
    Task RecordAsync(
        string action,
        string module,
        string resource,
        string? resourceId = null,
        object? oldValues = null,
        object? newValues = null,
        bool isSuccess = true,
        string? errorMessage = null,
        CancellationToken cancellationToken = default);
}
