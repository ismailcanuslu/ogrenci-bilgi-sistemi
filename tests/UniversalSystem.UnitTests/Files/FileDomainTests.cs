using Files.Domain.Errors;
using Files.Domain.Files;
using Files.Domain.ValueObjects;
using Xunit;
using File = Files.Domain.Files.File;

namespace UniversalSystem.UnitTests.Files;

public class FileDomainTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnPendingFile()
    {
        // Arrange
        var name = FileName.Create("document.pdf").Value!;
        var contentType = ContentType.Create("application/pdf").Value!;
        var size = FileSize.Create(1024).Value!;
        var now = DateTimeOffset.UtcNow;

        // Act
        var file = File.Create(name, contentType, size, now);

        // Assert
        Assert.NotNull(file);
        Assert.Equal(FileStatus.Pending, file.Status);
        Assert.Equal("document.pdf", file.Name.Value);
        Assert.Equal("application/pdf", file.ContentType.Value);
        Assert.Equal(1024, file.Size.Bytes);
        Assert.Null(file.Hash);
        Assert.Null(file.StorageKey);
    }

    [Fact]
    public void Activate_WithoutHashOrStorageKey_ShouldFail()
    {
        // Arrange
        var name = FileName.Create("document.pdf").Value!;
        var contentType = ContentType.Create("application/pdf").Value!;
        var size = FileSize.Create(1024).Value!;
        var file = File.Create(name, contentType, size, DateTimeOffset.UtcNow);

        // Act
        var result = file.Activate();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Code == FileErrors.HashRequiredForActivation.Code);
    }

    [Fact]
    public void Activate_WithHashAndStorageKey_ShouldSucceed()
    {
        // Arrange
        var name = FileName.Create("document.pdf").Value!;
        var contentType = ContentType.Create("application/pdf").Value!;
        var size = FileSize.Create(1024).Value!;
        var file = File.Create(name, contentType, size, DateTimeOffset.UtcNow);

        var hash = FileHash.Create(
            new string('a', 64),
            HashAlgorithm.Sha256).Value!;
        var storageKey = StorageKey.Create("2026/10/12/abc.pdf").Value!;

        file.SetHash(hash);
        file.SetStorageKey(storageKey);

        // Act
        var result = file.Activate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(FileStatus.Active, file.Status);
    }

    [Fact]
    public void Delete_ActiveFile_ShouldSetDeletedStatus()
    {
        // Arrange
        var name = FileName.Create("document.pdf").Value!;
        var contentType = ContentType.Create("application/pdf").Value!;
        var size = FileSize.Create(1024).Value!;
        var file = File.Create(name, contentType, size, DateTimeOffset.UtcNow);
        var hash = FileHash.Create(new string('a', 64), HashAlgorithm.Sha256).Value!;
        var storageKey = StorageKey.Create("2026/10/12/abc.pdf").Value!;

        file.SetHash(hash);
        file.SetStorageKey(storageKey);
        file.Activate();

        var deleteTime = DateTimeOffset.UtcNow;

        // Act
        var result = file.Delete(deleteTime);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(FileStatus.Deleted, file.Status);
        Assert.Equal(deleteTime, file.DeletedAt);
    }
}
