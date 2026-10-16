using BuildingBlocks.Application.Results;
using Files.Domain.Errors;

namespace Files.Domain.ValueObjects;

public sealed record FileHash
{
    public string Value { get; }

    public HashAlgorithm Algorithm { get; }

    private FileHash(
        string value,
        HashAlgorithm algorithm)
    {
        Value = value;
        Algorithm = algorithm;
    }

    public static Result<FileHash> Create(
        string? value,
        HashAlgorithm algorithm)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<FileHash>.Failure(
                FileErrors.InvalidHash);
        }

        var normalized = value.Trim().ToLowerInvariant();

        var expectedLength = algorithm switch
        {
            HashAlgorithm.Sha256 => 64,
            HashAlgorithm.Sha512 => 128,
            _ => 0
        };

        if (expectedLength == 0 ||
            normalized.Length != expectedLength ||
            !IsHexadecimal(normalized))
        {
            return Result<FileHash>.Failure(
                FileErrors.InvalidHash);
        }

        return Result<FileHash>.Success(
            new FileHash(normalized, algorithm));
    }

    private static bool IsHexadecimal(string value)
    {
        foreach (var character in value)
        {
            if (character is >= '0' and <= '9' ||
                character is >= 'a' and <= 'f')
            {
                continue;
            }

            return false;
        }

        return true;
    }

    public override string ToString()
        => $"{Algorithm}:{Value}";
}