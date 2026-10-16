using System.Text.RegularExpressions;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;
using Security.Domain.Errors;

namespace Security.Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex E164Regex =
        new(
            @"^\+[1-9]\d{7,14}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                SecurityErrors.PhoneNumber.Required);
        }

        var normalizedValue = value.Trim();

        if (!E164Regex.IsMatch(normalizedValue))
        {
            throw new DomainException(
                SecurityErrors.PhoneNumber.Invalid);
        }

        return new PhoneNumber(normalizedValue);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}