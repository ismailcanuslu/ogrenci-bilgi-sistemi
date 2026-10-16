using Files.Domain.ValueObjects;

namespace Files.Application.Abstractions.Storage;

public interface IFileStorageReader
{
    Task<Stream> OpenReadStreamAsync(
        StorageKey key,
        CancellationToken cancellationToken = default);
}