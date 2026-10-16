using SupportDesk.Domain.Tickets;

namespace SupportDesk.Application.Abstractions;

public interface ISupportTicketRepository
{
    Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SupportTicket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SupportTicket>> ListAsync(Guid? requesterUserId, SupportUnit? targetUnit, TicketStatus? status, CancellationToken cancellationToken = default);
    Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken = default);
}
