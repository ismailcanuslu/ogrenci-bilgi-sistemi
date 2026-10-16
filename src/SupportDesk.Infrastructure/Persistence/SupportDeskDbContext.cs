using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Abstractions;
using SupportDesk.Domain.Tickets;

namespace SupportDesk.Infrastructure.Persistence;

public sealed class SupportDeskDbContext(DbContextOptions<SupportDeskDbContext> options)
    : DbContext(options), ISupportDeskUnitOfWork
{
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("support_desk");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportDeskDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
