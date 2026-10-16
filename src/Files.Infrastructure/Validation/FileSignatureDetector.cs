using Files.Application.Abstractions.Validation;
using Files.Domain.ValueObjects;

namespace Files.Infrastructure.Validation;

public sealed class FileSignatureDetector : IFileSignatureDetector
{
    private static readonly Dictionary<string, List<byte[]>> Signatures = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] =
        [
            [0xFF, 0xD8, 0xFF]
        ],
        ["image/png"] =
        [
            [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]
        ],
        ["image/gif"] =
        [
            [0x47, 0x49, 0x46, 0x38, 0x37, 0x61],
            [0x47, 0x49, 0x46, 0x38, 0x39, 0x61]
        ],
        ["application/pdf"] =
        [
            [0x25, 0x50, 0x44, 0x46] // %PDF
        ],
        ["application/zip"] =
        [
            [0x50, 0x4B, 0x03, 0x04],
            [0x50, 0x4B, 0x05, 0x06]
        ],
        ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"] =
        [
            [0x50, 0x4B, 0x03, 0x04]
        ],
        ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"] =
        [
            [0x50, 0x4B, 0x03, 0x04]
        ]
    };

    public bool MatchesSignature(
        Stream stream,
        ContentType contentType,
        FileName fileName)
    {
        if (!Signatures.TryGetValue(contentType.Value, out var expectedSignatures))
        {
            // For general formats without strict magic numbers (e.g. text/plain, application/json),
            // we allow them as long as they don't spoof known binary signatures.
            return true;
        }

        var header = ReadHeader(stream, 32);

        foreach (var signature in expectedSignatures)
        {
            if (header.Length >= signature.Length &&
                header.AsSpan(0, signature.Length).SequenceEqual(signature))
            {
                return true;
            }
        }

        return false;
    }

    public ContentType? DetectContentType(Stream stream)
    {
        var header = ReadHeader(stream, 32);

        foreach (var (mime, signatures) in Signatures)
        {
            foreach (var signature in signatures)
            {
                if (header.Length >= signature.Length &&
                    header.AsSpan(0, signature.Length).SequenceEqual(signature))
                {
                    var result = ContentType.Create(mime);
                    if (result.IsSuccess)
                    {
                        return result.Value;
                    }
                }
            }
        }

        return null;
    }

    private static byte[] ReadHeader(Stream stream, int maxBytes)
    {
        if (!stream.CanSeek)
        {
            return [];
        }

        var initialPosition = stream.Position;
        stream.Position = 0;

        var buffer = new byte[maxBytes];
        var bytesRead = stream.Read(buffer, 0, maxBytes);

        stream.Position = initialPosition;

        if (bytesRead == maxBytes)
        {
            return buffer;
        }

        var result = new byte[bytesRead];
        Array.Copy(buffer, result, bytesRead);
        return result;
    }
}
