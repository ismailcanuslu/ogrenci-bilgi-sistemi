using Messaging.Application.Abstractions;
using Messaging.Domain;
using Microsoft.EntityFrameworkCore;

namespace Messaging.Infrastructure.Persistence;

public sealed class OutboxRepository(MessagingDbContext dbContext) : IOutboxRepository
{
    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await dbContext.OutboxMessages.AddAsync(message, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedBatchAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await dbContext.OutboxMessages
            .Where(x => x.ProcessedOnUtc == null && x.RetryCount < 5)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        dbContext.OutboxMessages.Update(message);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<OutboxMessage> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        bool? processedOnly = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<OutboxMessage> query = dbContext.OutboxMessages.AsNoTracking();

        if (processedOnly.HasValue)
        {
            query = processedOnly.Value
                ? query.Where(x => x.ProcessedOnUtc != null)
                : query.Where(x => x.ProcessedOnUtc == null);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.OccurredOnUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
