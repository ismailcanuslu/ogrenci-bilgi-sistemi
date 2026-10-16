using Files.Domain.Files;
using Files.Domain.ValueObjects;
using File = Files.Domain.Files.File;

namespace Files.Application.Abstractions.Persistence;

public interface IFileRepository
{
    Task<File?> GetByIdAsync(FileId id, CancellationToken cancellationToken = default);

    Task<File?> GetByStorageKeyAsync(StorageKey storageKey, CancellationToken cancellationToken = default);

    Task AddAsync(File file, CancellationToken cancellationToken = default);

    void Update(File file);
}
