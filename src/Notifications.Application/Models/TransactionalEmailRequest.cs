namespace Notifications.Application.Models;

public sealed record TransactionalEmailRequest(
    string To,
    string Subject,
    string Heading,
    string? Message = null,
    string? Preheader = null,
    string? OtpCode = null,
    string? ButtonText = null,
    string? ButtonUrl = null,
    int? ExpiresInMinutes = null,
    string? Disclaimer = null,
    string? Theme = null);
