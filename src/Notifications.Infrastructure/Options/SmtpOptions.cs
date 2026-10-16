namespace Notifications.Infrastructure.Options;

public sealed class SmtpOptions
{
    public const string SectionName = "Notifications:Smtp";

    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 1025;

    public bool EnableSsl { get; set; } = false;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string FromEmail { get; set; } = "noreply@obs-platform.local";

    public string FromName { get; set; } = "OBS Platform";
}
