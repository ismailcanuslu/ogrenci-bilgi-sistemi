using System.Security.Cryptography;
using Files.Application.Abstractions.Security;
using Files.Domain.ValueObjects;
using HashAlgorithm = Files.Domain.ValueObjects.HashAlgorithm;

namespace Files.Infrastructure.Security;

public sealed class FileHashCalculator : IFileHashCalculator
{
    public async Task<FileHash> ComputeHashAsync(
        Stream stream,
        HashAlgorithm algorithm = HashAlgorithm.Sha256,
        CancellationToken cancellationToken = default)
    {
        var initialPosition = stream.CanSeek ? stream.Position : 0;
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        byte[] hashBytes;
        if (algorithm is HashAlgorithm.Sha512)
        {
            hashBytes = await SHA512.HashDataAsync(stream, cancellationToken);
        }
        else
        {
            hashBytes = await SHA256.HashDataAsync(stream, cancellationToken);
        }

        if (stream.CanSeek)
        {
            stream.Position = initialPosition;
        }

        var hexString = Convert.ToHexStringLower(hashBytes);
        var result = FileHash.Create(hexString, algorithm);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException("Failed to create FileHash from computed hash bytes.");
        }

        return result.Value!;
    }
}
