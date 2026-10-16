using BuildingBlocks.Application.Results;
using Files.Domain.ValueObjects;

namespace Files.Application.Abstractions.Validation;

public interface IFileValidator
{
    Task<Result> ValidateAsync(
        Stream stream,
        FileName fileName,
        ContentType claimedContentType,
        FileSize fileSize,
        CancellationToken cancellationToken = default);
}