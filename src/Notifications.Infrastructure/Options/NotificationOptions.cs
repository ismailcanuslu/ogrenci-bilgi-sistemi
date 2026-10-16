namespace Notifications.Infrastructure.Options;

public sealed class NotificationOptions
{
    public const string SectionName = "Notifications";

    /// <summary>
    /// Email provider: "Smtp" (default) or "Console"
    /// </summary>
    public string Provider { get; set; } = "Smtp";

    public SmtpOptions Smtp { get; set; } = new();

    public NotificationBrandingOptions Branding { get; set; } = new();

    public SmsOptions Sms { get; set; } = new();
}
