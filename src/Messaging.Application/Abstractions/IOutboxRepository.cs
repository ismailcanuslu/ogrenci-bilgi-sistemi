using Messaging.Domain;

namespace Messaging.Application.Abstractions;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedBatchAsync(int batchSize, CancellationToken cancellationToken = default);

    Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<OutboxMessage> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        bool? processedOnly = null,
        CancellationToken cancellationToken = default);
}
