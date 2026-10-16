using System.Buffers;
using System.Text;
using BuildingBlocks.Application.Results;
using Files.Domain.Errors;

namespace Files.Domain.ValueObjects;

public sealed record FileName
{
    private const int MaxLength = 255;

    private static readonly SearchValues<char> InvalidCharacters =
        SearchValues.Create(
            "<>:\"/\\|?*");

    public string Value { get; }

    private FileName(string value)
    {
        Value = value;
    }

    public static Result<FileName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<FileName>.Failure(
                FileErrors.InvalidName);
        }

        var normalized = value.Normalize(NormalizationForm.FormC).Trim();

        if (normalized.Length == 0 ||
            normalized.Length > MaxLength)
        {
            return Result<FileName>.Failure(
                FileErrors.InvalidName);
        }

        if (normalized is "." or "..")
        {
            return Result<FileName>.Failure(
                FileErrors.InvalidName);
        }

        if (normalized.AsSpan().IndexOfAny(InvalidCharacters) >= 0)
        {
            return Result<FileName>.Failure(
                FileErrors.InvalidName);
        }

        foreach (var character in normalized)
        {
            if (char.IsControl(character))
            {
                return Result<FileName>.Failure(
                    FileErrors.InvalidName);
            }
        }

        if (normalized[^1] is '.' or ' ')
        {
            return Result<FileName>.Failure(
                FileErrors.InvalidName);
        }

        if (IsReservedDeviceName(normalized))
        {
            return Result<FileName>.Failure(
                FileErrors.InvalidName);
        }

        return Result<FileName>.Success(
            new FileName(normalized));
    }

    private static bool IsReservedDeviceName(string value)
    {
        var nameWithoutExtension = value;

        var extensionIndex = value.IndexOf('.');

        if (extensionIndex >= 0)
        {
            nameWithoutExtension = value[..extensionIndex];
        }

        return nameWithoutExtension.ToUpperInvariant() switch
        {
            "CON" => true,
            "PRN" => true,
            "AUX" => true,
            "NUL" => true,
            "COM1" => true,
            "COM2" => true,
            "COM3" => true,
            "COM4" => true,
            "COM5" => true,
            "COM6" => true,
            "COM7" => true,
            "COM8" => true,
            "COM9" => true,
            "LPT1" => true,
            "LPT2" => true,
            "LPT3" => true,
            "LPT4" => true,
            "LPT5" => true,
            "LPT6" => true,
            "LPT7" => true,
            "LPT8" => true,
            "LPT9" => true,
            _ => false
        };
    }

    public override string ToString() => Value;
}