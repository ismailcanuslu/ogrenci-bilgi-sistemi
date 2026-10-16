using Files.Domain.ValueObjects;

namespace Files.Application.Abstractions.Security;

public interface IFileHashCalculator
{
    Task<FileHash> ComputeHashAsync(
        Stream stream,
        HashAlgorithm algorithm = HashAlgorithm.Sha256,
        CancellationToken cancellationToken = default);
}
