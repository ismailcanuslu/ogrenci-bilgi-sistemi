using Files.Domain.ValueObjects;

namespace Files.Application.Abstractions.Validation;

public interface IFileSignatureDetector
{
    bool MatchesSignature(
        Stream stream,
        ContentType contentType,
        FileName fileName);

    ContentType? DetectContentType(Stream stream);
}