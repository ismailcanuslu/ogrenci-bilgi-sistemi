namespace BuildingBlocks.Application.Results;

public sealed record Error(
    string Code,
    ErrorType Type,
    string? Path = null);