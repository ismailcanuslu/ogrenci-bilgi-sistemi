using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webhooks.Application.Abstractions;
using Webhooks.Infrastructure.Persistence;
using Webhooks.Infrastructure.Services;

namespace Webhooks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWebhooksInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<WebhooksDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "webhooks");
            });
        });

        services.AddScoped<IWebhookRepository, WebhookRepository>();
        services.AddSingleton<IWebhookSigner, HmacWebhookSigner>();
        services.AddScoped<IWebhookDispatcher, WebhookDispatcher>();

        services.AddHttpClient("WebhooksClient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("User-Agent", "OBS-Webhook-Dispatcher/1.0");
        });

        return services;
    }
}
