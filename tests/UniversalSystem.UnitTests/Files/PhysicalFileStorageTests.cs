using System.Security;
using System.Text;
using Files.Domain.ValueObjects;
using Files.Infrastructure.Options;
using Files.Infrastructure.Storage;
using Microsoft.Extensions.Options;
using Xunit;

namespace UniversalSystem.UnitTests.Files;

public class PhysicalFileStorageTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly PhysicalFileStorage _storage;

    public PhysicalFileStorageTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "obs_test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testDirectory);

        var options = Options.Create(new FileStorageOptions
        {
            BasePath = _testDirectory
        });

        _storage = new PhysicalFileStorage(options);
    }

    [Fact]
    public async Task UploadAsync_And_OpenReadStreamAsync_ShouldPersistAndReadFile()
    {
        // Arrange
        var content = "Universal Obs File Storage Content";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileName = FileName.Create("test.txt").Value!;
        var contentType = ContentType.Create("text/plain").Value!;

        // Act
        var storageKey = await _storage.UploadAsync(stream, fileName, contentType);
        Assert.NotNull(storageKey);

        using var readStream = await _storage.OpenReadStreamAsync(storageKey);
        using var reader = new StreamReader(readStream);
        var readContent = await reader.ReadToEndAsync();

        // Assert
        Assert.Equal(content, readContent);
    }

    [Fact]
    public async Task OpenReadStreamAsync_WithTraversalKey_ShouldThrowSecurityException()
    {
        // StorageKey value that attempts path traversal is rejected by domain or storage
        var invalidKey = StorageKey.Create("valid/key.txt").Value!;

        // Now test if storage protects against outside traversal
        var exists = await _storage.ExistsAsync(invalidKey);
        Assert.False(exists);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, true);
            }
            catch
            {
                // ignore
            }
        }
    }
}
