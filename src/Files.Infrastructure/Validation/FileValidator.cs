using BuildingBlocks.Application.Results;
using Files.Application.Abstractions.Validation;
using Files.Domain.ValueObjects;
using Files.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Files.Infrastructure.Validation;

public sealed class FileValidator(
    IOptions<FileValidationOptions> options,
    IFileSignatureDetector signatureDetector)
    : IFileValidator
{
    public Task<Result> ValidateAsync(
        Stream stream,
        FileName fileName,
        ContentType claimedContentType,
        FileSize fileSize,
        CancellationToken cancellationToken = default)
    {
        var config = options.Value;

        if (fileSize.Bytes <= 0)
        {
            return Task.FromResult(
                Result.Failure(
                    new Error(
                        "Files.EmptyFile",
                        ErrorType.Validation)));
        }

        if (config.MaxFileSizeInBytes > 0 && fileSize.Bytes > config.MaxFileSizeInBytes)
        {
            return Task.FromResult(
                Result.Failure(
                    new Error(
                        "Files.FileTooLarge",
                        ErrorType.Validation)));
        }

        if (config.AllowedContentTypes is { Length: > 0 } &&
            !config.AllowedContentTypes.Contains(claimedContentType.Value, StringComparer.OrdinalIgnoreCase))
        {
            return Task.FromResult(
                Result.Failure(
                    new Error(
                        "Files.UnsupportedContentType",
                        ErrorType.Validation)));
        }

        var extension = Path.GetExtension(fileName.Value).ToLowerInvariant();
        if (config.AllowedExtensions is { Length: > 0 } &&
            !config.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            return Task.FromResult(
                Result.Failure(
                    new Error(
                        "Files.UnsupportedExtension",
                        ErrorType.Validation)));
        }

        var matches = signatureDetector.MatchesSignature(stream, claimedContentType, fileName);
        if (!matches)
        {
            return Task.FromResult(
                Result.Failure(
                    new Error(
                        "Files.SignatureMismatch",
                        ErrorType.Validation)));
        }

        return Task.FromResult(Result.Success());
    }
}
