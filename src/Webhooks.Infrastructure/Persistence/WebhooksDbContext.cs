using Microsoft.EntityFrameworkCore;
using Webhooks.Domain;
using Webhooks.Infrastructure.Persistence.Configurations;

namespace Webhooks.Infrastructure.Persistence;

public sealed class WebhooksDbContext : DbContext
{
    public DbSet<WebhookSubscription> Subscriptions => Set<WebhookSubscription>();
    public DbSet<WebhookDeliveryLog> DeliveryLogs => Set<WebhookDeliveryLog>();

    public WebhooksDbContext(DbContextOptions<WebhooksDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebhooksDbContext).Assembly);
    }
}
