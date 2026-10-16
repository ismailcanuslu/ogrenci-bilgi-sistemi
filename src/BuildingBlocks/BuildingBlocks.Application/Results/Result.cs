using BuildingBlocks.Application.Results;

namespace BuildingBlocks.Application.Results;

public class Result
{
    private protected Result(
        bool isSuccess,
        IEnumerable<Error>? errors)
    {
        IsSuccess = isSuccess;
        Errors = errors?.ToArray() ?? [];
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public IReadOnlyCollection<Error> Errors { get; }

    public static Result Success()
        => new(
            isSuccess: true,
            errors: null);

    public static Result Failure(params Error[] errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Length == 0)
        {
            throw new ArgumentException(
                "A failure result must contain at least one error.",
                nameof(errors));
        }

        return new Result(
            isSuccess: false,
            errors: errors);
    }
}

public sealed class Result<T> : Result
{
    private Result(
        bool isSuccess,
        T? value,
        IEnumerable<Error>? errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value)
        => new(
            isSuccess: true,
            value: value,
            errors: null);

    public static new Result<T> Failure(params Error[] errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Length == 0)
        {
            throw new ArgumentException(
                "A failure result must contain at least one error.",
                nameof(errors));
        }

        return new Result<T>(
            isSuccess: false,
            value: default,
            errors: errors);
    }
}