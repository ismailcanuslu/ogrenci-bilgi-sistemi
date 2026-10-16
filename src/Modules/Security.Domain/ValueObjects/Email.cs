using System.Net.Mail;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;
using Security.Domain.Errors;

namespace Security.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                SecurityErrors.Email.Required);
        }

        var normalizedValue = value.Trim().ToLowerInvariant();

        if (!IsValid(normalizedValue))
        {
            throw new DomainException(
                SecurityErrors.Email.Invalid);
        }

        return new Email(normalizedValue);
    }

    private static bool IsValid(string value)
    {
        try
        {
            var address = new MailAddress(value);

            return string.Equals(
                address.Address,
                value,
                StringComparison.Ordinal);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}