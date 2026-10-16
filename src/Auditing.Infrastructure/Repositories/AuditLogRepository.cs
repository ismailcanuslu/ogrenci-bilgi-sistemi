using Auditing.Application.Abstractions;
using Auditing.Domain;
using Auditing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auditing.Infrastructure.Repositories;

public sealed class AuditLogRepository(AuditingDbContext dbContext) : IAuditLogRepository
{
    public async Task<AuditLog?> GetByIdAsync(AuditLogId id, CancellationToken cancellationToken = default)
    {
        return await dbContext.AuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? module = null,
        string? action = null,
        Guid? userId = null,
        string? resource = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLog> query = dbContext.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(module))
        {
            query = query.Where(x => x.Module == module);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(x => x.Action == action);
        }

        if (userId.HasValue)
        {
            query = query.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(resource))
        {
            query = query.Where(x => x.Resource == resource);
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.Timestamp <= to.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
