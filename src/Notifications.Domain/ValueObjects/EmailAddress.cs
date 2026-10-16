using System.Net.Mail;
using BuildingBlocks.Application.Results;
using Notifications.Domain.Errors;

namespace Notifications.Domain.ValueObjects;

public sealed record EmailAddress
{
    public const int MaxLength = 256;

    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    private EmailAddress()
    {
        Value = string.Empty;
    }

    public static Result<EmailAddress> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<EmailAddress>.Failure(NotificationErrors.InvalidEmail);
        }

        var trimmed = value.Trim().ToLowerInvariant();

        if (trimmed.Length > MaxLength)
        {
            return Result<EmailAddress>.Failure(NotificationErrors.InvalidEmail);
        }

        try
        {
            var parsed = new MailAddress(trimmed);
            if (parsed.Address != trimmed || !trimmed.Contains('.'))
            {
                return Result<EmailAddress>.Failure(NotificationErrors.InvalidEmail);
            }
        }
        catch (FormatException)
        {
            return Result<EmailAddress>.Failure(NotificationErrors.InvalidEmail);
        }

        return Result<EmailAddress>.Success(new EmailAddress(trimmed));
    }

    public override string ToString() => Value;
}
