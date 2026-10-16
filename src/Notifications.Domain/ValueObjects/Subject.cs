using BuildingBlocks.Application.Results;
using Notifications.Domain.Errors;

namespace Notifications.Domain.ValueObjects;

public sealed record Subject
{
    public const int MaxLength = 255;

    public string Value { get; }

    private Subject(string value)
    {
        Value = value;
    }

    private Subject()
    {
        Value = string.Empty;
    }

    public static Result<Subject> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Subject>.Failure(NotificationErrors.InvalidSubject);
        }

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result<Subject>.Failure(NotificationErrors.InvalidSubject);
        }

        return Result<Subject>.Success(new Subject(trimmed));
    }

    public override string ToString() => Value;
}
