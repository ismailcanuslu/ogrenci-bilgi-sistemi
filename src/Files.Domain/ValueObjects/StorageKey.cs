using BuildingBlocks.Application.Results;
using Files.Domain.Errors;

namespace Files.Domain.ValueObjects;

public sealed record StorageKey
{
    private const int MaxLength = 1024;

    public string Value { get; }

    private StorageKey(string value)
    {
        Value = value;
    }

    public static Result<StorageKey> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<StorageKey>.Failure(
                FileErrors.InvalidStorageKey);
        }

        var normalized = value.Trim();

        if (normalized.Length > MaxLength)
        {
            return Result<StorageKey>.Failure(
                FileErrors.InvalidStorageKey);
        }

        if (normalized.StartsWith('/') ||
            normalized.EndsWith('/'))
        {
            return Result<StorageKey>.Failure(
                FileErrors.InvalidStorageKey);
        }

        foreach (var character in normalized)
        {
            if (char.IsControl(character) ||
                character == '\\' ||
                character == '\0')
            {
                return Result<StorageKey>.Failure(
                    FileErrors.InvalidStorageKey);
            }
        }

        var segments = normalized.Split('/');

        foreach (var segment in segments)
        {
            if (segment is "." or "..")
            {
                return Result<StorageKey>.Failure(
                    FileErrors.InvalidStorageKey);
            }
        }

        return Result<StorageKey>.Success(
            new StorageKey(normalized));
    }

    public override string ToString() => Value;
}