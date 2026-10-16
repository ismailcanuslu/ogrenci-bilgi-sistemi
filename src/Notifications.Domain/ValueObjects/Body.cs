using BuildingBlocks.Application.Results;
using Notifications.Domain.Errors;

namespace Notifications.Domain.ValueObjects;

public sealed record Body
{
    public string Value { get; }

    public bool IsHtml { get; }

    private Body(string value, bool isHtml)
    {
        Value = value;
        IsHtml = isHtml;
    }

    private Body()
    {
        Value = string.Empty;
        IsHtml = false;
    }

    public static Result<Body> Create(string? value, bool isHtml = false)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Body>.Failure(NotificationErrors.InvalidBody);
        }

        return Result<Body>.Success(new Body(value.Trim(), isHtml));
    }

    public override string ToString() => Value;
}
