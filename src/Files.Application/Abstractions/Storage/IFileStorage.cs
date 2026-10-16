using Files.Domain.ValueObjects;

namespace Files.Application.Abstractions.Storage;

public interface IFileStorage
{
    Task<StorageKey> UploadAsync(
        Stream contentStream,
        FileName fileName,
        ContentType contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        StorageKey key,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        StorageKey key,
        CancellationToken cancellationToken = default);
}