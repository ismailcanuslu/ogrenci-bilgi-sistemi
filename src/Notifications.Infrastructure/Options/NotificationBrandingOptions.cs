namespace Notifications.Infrastructure.Options;

public sealed class NotificationBrandingOptions
{
    public const string SectionName = "Notifications:Branding";

    public string CompanyName { get; set; } = "Example University";

    public string LogoUrl { get; set; } = "https://example.com/assets/logo.png";

    public string SupportUrl { get; set; } = "https://support.example.com";

    public string SupportEmail { get; set; } = "support@example.com";

    public string CompanyAddress { get; set; } = "Example University Campus, Istanbul";

    public string PrimaryColor { get; set; } = "#6166BE";

    public string AccentColor { get; set; } = "#F0F0F9";

    /// <summary>
    /// Default email theme: "ModernCard" (RedotPay / Paybis style) or "Clean" (Stripe style)
    /// </summary>
    public string DefaultTheme { get; set; } = "ModernCard";
}
