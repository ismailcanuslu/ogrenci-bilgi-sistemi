using BuildingBlocks.Application.Abstractions.Persistence;
using Messaging.Application.Abstractions;
using Messaging.Infrastructure.Background;
using Messaging.Infrastructure.Bus;
using Messaging.Infrastructure.Persistence;
using Messaging.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMessagingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<MessagingDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<MessagingDbContext>());

        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddSingleton<IMessageBus, ChannelMessageBus>();
        services.AddScoped<IEventPublisher, EventPublisher>();

        services.AddHostedService<OutboxProcessorBackgroundService>();

        return services;
    }
}
