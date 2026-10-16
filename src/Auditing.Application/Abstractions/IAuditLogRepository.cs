using Auditing.Domain;

namespace Auditing.Application.Abstractions;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(AuditLogId id, CancellationToken cancellationToken = default);

    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? module = null,
        string? action = null,
        Guid? userId = null,
        string? resource = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default);
}
