using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Abstractions;
using SupportDesk.Domain.Tickets;

namespace SupportDesk.Infrastructure.Persistence.Repositories;

public sealed class SupportTicketRepository(SupportDeskDbContext dbContext) : ISupportTicketRepository
{
    public Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.SupportTickets
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SupportTicket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken = default)
        => dbContext.SupportTickets
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(x => x.TicketNumber == ticketNumber, cancellationToken);

    public async Task<IReadOnlyCollection<SupportTicket>> ListAsync(
        Guid? requesterUserId, SupportUnit? targetUnit, TicketStatus? status, CancellationToken cancellationToken = default)
    {
        var query = dbContext.SupportTickets
            .Include(x => x.Messages)
            .AsQueryable();

        if (requesterUserId.HasValue)
        {
            query = query.Where(x => x.RequesterUserId == requesterUserId.Value);
        }

        if (targetUnit.HasValue)
        {
            query = query.Where(x => x.TargetUnit == targetUnit.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken = default)
        => await dbContext.SupportTickets.AddAsync(ticket, cancellationToken);
}
