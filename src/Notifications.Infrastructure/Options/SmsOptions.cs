namespace Notifications.Infrastructure.Options;

public sealed class SmsOptions
{
    public const string SectionName = "Notifications:Sms";

    /// <summary>
    /// SMS provider: "Console" (default/test) or "Http" (generic gateway/webhook)
    /// </summary>
    public string Provider { get; set; } = "Console";

    public string SenderHeader { get; set; } = "OBSPLATFORM";

    public string? EndpointUrl { get; set; }

    public string? ApiKey { get; set; }

    public string? ApiSecret { get; set; }
}
