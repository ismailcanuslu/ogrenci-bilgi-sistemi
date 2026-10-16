using BuildingBlocks.Application.Abstractions.Persistence;
using Messaging.Domain;
using Microsoft.EntityFrameworkCore;

namespace Messaging.Infrastructure.Persistence;

public sealed class MessagingDbContext : DbContext, IUnitOfWork
{
    public MessagingDbContext(DbContextOptions<MessagingDbContext> options)
        : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessagingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
