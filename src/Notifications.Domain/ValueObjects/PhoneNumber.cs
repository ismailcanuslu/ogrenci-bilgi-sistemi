using System.Text.RegularExpressions;
using BuildingBlocks.Application.Results;
using Notifications.Domain.Errors;

namespace Notifications.Domain.ValueObjects;

public sealed record PhoneNumber
{
    private static readonly Regex PhoneRegex = new(
        @"^\+?[1-9]\d{9,14}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    private PhoneNumber()
    {
        Value = string.Empty;
    }

    public static Result<PhoneNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<PhoneNumber>.Failure(NotificationErrors.InvalidPhoneNumber);
        }

        var cleaned = value.Trim().Replace(" ", "").Replace("-", "");

        if (!PhoneRegex.IsMatch(cleaned))
        {
            return Result<PhoneNumber>.Failure(NotificationErrors.InvalidPhoneNumber);
        }

        return Result<PhoneNumber>.Success(new PhoneNumber(cleaned));
    }

    public override string ToString() => Value;
}
