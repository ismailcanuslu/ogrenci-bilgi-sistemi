using BuildingBlocks.Application.Results;
using Files.Domain.Errors;

namespace Files.Domain.ValueObjects;

public sealed record ContentType
{
    private const int MaxLength = 255;

    public string Value { get; }

    private ContentType(string value)
    {
        Value = value;
    }

    public static Result<ContentType> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<ContentType>.Failure(
                FileErrors.InvalidContentType);
        }

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > MaxLength)
        {
            return Result<ContentType>.Failure(
                FileErrors.InvalidContentType);
        }

        var separatorIndex = normalized.IndexOf('/');

        if (separatorIndex <= 0 ||
            separatorIndex != normalized.LastIndexOf('/') ||
            separatorIndex == normalized.Length - 1)
        {
            return Result<ContentType>.Failure(
                FileErrors.InvalidContentType);
        }

        var type = normalized[..separatorIndex];
        var subtype = normalized[(separatorIndex + 1)..];

        if (type is "*" || subtype is "*")
        {
            return Result<ContentType>.Failure(
                FileErrors.InvalidContentType);
        }

        if (!IsValidToken(type) || !IsValidToken(subtype))
        {
            return Result<ContentType>.Failure(
                FileErrors.InvalidContentType);
        }

        return Result<ContentType>.Success(
            new ContentType(normalized));
    }

    private static bool IsValidToken(string value)
    {
        foreach (var character in value)
        {
            if (char.IsAsciiLetterOrDigit(character))
            {
                continue;
            }

            if (character is
                '!' or '#' or '$' or '%' or '&' or '\'' or
                '*' or '+' or '-' or '.' or '^' or '_' or
                '`' or '|' or '~')
            {
                continue;
            }

            return false;
        }

        return true;
    }

    public override string ToString() => Value;
}