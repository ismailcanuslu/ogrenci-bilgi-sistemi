using BuildingBlocks.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Notifications.Application.Abstractions.Infrastructure;
using Notifications.Application.Abstractions.Persistence;
using Notifications.Application.Abstractions.Templates;
using Notifications.Infrastructure.Email;
using Notifications.Infrastructure.Options;
using Notifications.Infrastructure.Persistence;
using Notifications.Infrastructure.Persistence.Repositories;
using Notifications.Infrastructure.Sms;
using Notifications.Infrastructure.Templates;

namespace Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<NotificationsDbContext>());

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddSingleton<ITemplateRenderer, SimpleTemplateRenderer>();

        services
            .AddOptions<NotificationOptions>()
            .Bind(configuration.GetSection(NotificationOptions.SectionName));

        services
            .AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName));

        services
            .AddOptions<NotificationBrandingOptions>()
            .Bind(configuration.GetSection(NotificationBrandingOptions.SectionName));

        services
            .AddOptions<SmsOptions>()
            .Bind(configuration.GetSection(SmsOptions.SectionName));

        services.AddTransient<SmtpEmailSender>();
        services.AddTransient<ConsoleEmailSender>();

        services.AddTransient<IEmailSender>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<NotificationOptions>>().Value;
            return string.Equals(options.Provider, "Console", StringComparison.OrdinalIgnoreCase)
                ? provider.GetRequiredService<ConsoleEmailSender>()
                : provider.GetRequiredService<SmtpEmailSender>();
        });

        services.AddTransient<ConsoleSmsSender>();
        services.AddTransient<HttpSmsSender>(provider =>
        {
            var httpClient = provider.GetService<HttpClient>() ?? new HttpClient();
            var options = provider.GetRequiredService<IOptions<NotificationOptions>>();
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<HttpSmsSender>>();
            return new HttpSmsSender(httpClient, options, logger);
        });

        services.AddTransient<ISmsSender>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<NotificationOptions>>().Value;
            return string.Equals(options.Sms.Provider, "Http", StringComparison.OrdinalIgnoreCase)
                ? provider.GetRequiredService<HttpSmsSender>()
                : provider.GetRequiredService<ConsoleSmsSender>();
        });
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        services.AddScoped<ISmsNotificationService, SmsNotificationService>();

        return services;
    }
}
