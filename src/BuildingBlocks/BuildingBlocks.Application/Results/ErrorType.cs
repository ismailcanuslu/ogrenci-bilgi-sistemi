namespace BuildingBlocks.Application.Results;

public enum ErrorType
{
    None = 0,
    Validation,
    BadRequest,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    Failure
}