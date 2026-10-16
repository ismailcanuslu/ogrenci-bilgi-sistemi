using System.Security;
using Files.Application.Abstractions.Storage;
using Files.Domain.ValueObjects;
using Files.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Files.Infrastructure.Storage;

public sealed class PhysicalFileStorage : IFileStorage, IFileStorageReader
{
    private readonly string _basePath;

    public PhysicalFileStorage(IOptions<FileStorageOptions> options)
    {
        var rawPath = string.IsNullOrWhiteSpace(options.Value.BasePath)
            ? "uploads"
            : options.Value.BasePath;

        var fullPath = Path.GetFullPath(rawPath);
        _basePath = fullPath.EndsWith(Path.DirectorySeparatorChar.ToString())
            ? fullPath
            : fullPath + Path.DirectorySeparatorChar;

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<StorageKey> UploadAsync(
        Stream contentStream,
        FileName fileName,
        ContentType contentType,
        CancellationToken cancellationToken = default)
    {
        var safeFileName = Path.GetFileName(fileName.Value);
        var extension = Path.GetExtension(safeFileName).ToLowerInvariant();

        var datePrefix = DateTime.UtcNow.ToString("yyyy/MM/dd");
        var uniqueId = Guid.NewGuid().ToString("N");
        var keyString = $"{datePrefix}/{uniqueId}{extension}";

        var storageKeyResult = StorageKey.Create(keyString);
        if (!storageKeyResult.IsSuccess)
        {
            throw new InvalidOperationException(
                "Failed to generate a valid storage key.");
        }

        var destinationPath = GetValidatedFullPath(storageKeyResult.Value!);
        var destinationDir = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        await using var fileStream = new FileStream(
            destinationPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await contentStream.CopyToAsync(fileStream, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);

        return storageKeyResult.Value!;
    }

    public Task<Stream> OpenReadStreamAsync(
        StorageKey key,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetValidatedFullPath(key);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(
                "Physical file was not found on storage.",
                fullPath);
        }

        Stream fileStream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        return Task.FromResult(fileStream);
    }

    public Task DeleteAsync(
        StorageKey key,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetValidatedFullPath(key);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(
        StorageKey key,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetValidatedFullPath(key);
        return Task.FromResult(File.Exists(fullPath));
    }

    private string GetValidatedFullPath(StorageKey key)
    {
        var normalizedKey = key.Value.Replace('/', Path.DirectorySeparatorChar);
        var combined = Path.Combine(_basePath, normalizedKey);
        var resolvedFullPath = Path.GetFullPath(combined);

        if (!resolvedFullPath.StartsWith(_basePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityException(
                "Path traversal attempt detected. The storage path resolved outside the allowed base directory.");
        }

        return resolvedFullPath;
    }
}
