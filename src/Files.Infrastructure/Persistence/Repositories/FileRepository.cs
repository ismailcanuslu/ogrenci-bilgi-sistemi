using Files.Application.Abstractions.Persistence;
using Files.Domain.Files;
using Files.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using File = Files.Domain.Files.File;

namespace Files.Infrastructure.Persistence.Repositories;

public sealed class FileRepository(FilesDbContext context) : IFileRepository
{
    public async Task<File?> GetByIdAsync(
        FileId id,
        CancellationToken cancellationToken = default)
    {
        return await context.Files
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<File?> GetByStorageKeyAsync(
        StorageKey storageKey,
        CancellationToken cancellationToken = default)
    {
        return await context.Files
            .FirstOrDefaultAsync(
                x => x.StorageKey != null && x.StorageKey.Value == storageKey.Value,
                cancellationToken);
    }

    public async Task AddAsync(
        File file,
        CancellationToken cancellationToken = default)
    {
        await context.Files.AddAsync(file, cancellationToken);
    }

    public void Update(File file)
    {
        context.Files.Update(file);
    }
}
