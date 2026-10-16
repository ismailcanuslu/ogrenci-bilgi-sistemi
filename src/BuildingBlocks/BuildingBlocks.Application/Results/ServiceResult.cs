namespace BuildingBlocks.Application.Results;

public class ServiceResult<T>
{
    private ServiceResult(
        bool isSuccess,
        int statusCode,
        T? data,
        IEnumerable<Error>? errors)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Data = data;
        Errors = errors?.ToArray() ?? [];
    }

    public bool IsSuccess { get; }

    public int StatusCode { get; }

    public T? Data { get; }

    public IReadOnlyCollection<Error> Errors { get; }

    public static ServiceResult<T> Ok(T data)
        => new(
            isSuccess: true,
            statusCode: 200,
            data: data,
            errors: null);

    public static ServiceResult<T> Created(T data)
        => new(
            isSuccess: true,
            statusCode: 201,
            data: data,
            errors: null);

    public static ServiceResult<T> NoContent()
        => new(
            isSuccess: true,
            statusCode: 204,
            data: default,
            errors: null);

    public static ServiceResult<T> BadRequest(
        params Error[] errors)
        => Failure(400, errors);

    public static ServiceResult<T> Unauthorized(
        params Error[] errors)
        => Failure(401, errors);

    public static ServiceResult<T> Forbidden(
        params Error[] errors)
        => Failure(403, errors);

    public static ServiceResult<T> NotFound(
        params Error[] errors)
        => Failure(404, errors);

    public static ServiceResult<T> Conflict(
        params Error[] errors)
        => Failure(409, errors);

    public static ServiceResult<T> Failure(
        params Error[] errors)
        => Failure(500, errors);

    private static ServiceResult<T> Failure(
        int statusCode,
        params Error[] errors)
        => new(
            isSuccess: false,
            statusCode: statusCode,
            data: default,
            errors: errors);
}

public sealed class ServiceResult
{
    private ServiceResult(
        bool isSuccess,
        int statusCode,
        IEnumerable<Error>? errors)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Errors = errors?.ToArray() ?? [];
    }

    public bool IsSuccess { get; }

    public int StatusCode { get; }

    public IReadOnlyCollection<Error> Errors { get; }

    public static ServiceResult Ok()
        => new(
            isSuccess: true,
            statusCode: 200,
            errors: null);

    public static ServiceResult Created()
        => new(
            isSuccess: true,
            statusCode: 201,
            errors: null);

    public static ServiceResult NoContent()
        => new(
            isSuccess: true,
            statusCode: 204,
            errors: null);

    public static ServiceResult BadRequest(
        params Error[] errors)
        => Failure(400, errors);

    public static ServiceResult Unauthorized(
        params Error[] errors)
        => Failure(401, errors);

    public static ServiceResult Forbidden(
        params Error[] errors)
        => Failure(403, errors);

    public static ServiceResult NotFound(
        params Error[] errors)
        => Failure(404, errors);

    public static ServiceResult Conflict(
        params Error[] errors)
        => Failure(409, errors);

    public static ServiceResult Failure(
        params Error[] errors)
        => Failure(500, errors);

    private static ServiceResult Failure(
        int statusCode,
        params Error[] errors)
        => new(
            isSuccess: false,
            statusCode: statusCode,
            errors: errors);
}

