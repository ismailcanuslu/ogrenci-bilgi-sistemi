using Files.Domain.Files;
using Files.Domain.ValueObjects;
using Files.Infrastructure.Options;
using Files.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Xunit;
using File = Files.Domain.Files.File;

namespace UniversalSystem.UnitTests.Files;

public class FileAuthorizationServiceTests
{
    private static File CreateTestFile(Guid? ownerId = null)
    {
        var name = FileName.Create("document.pdf").Value!;
        var contentType = ContentType.Create("application/pdf").Value!;
        var size = FileSize.Create(1024).Value;
        return File.Create(name, contentType, size, DateTimeOffset.UtcNow, ownerId);
    }

    [Fact]
    public async Task CanUpload_WhenAuthorizationNotEnforced_ShouldReturnTrueEvenWithoutUser()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new FileSecurityOptions
        {
            EnforceAuthorization = false
        });
        var service = new FileAuthorizationService(options);

        // Act
        var result = await service.CanUploadAsync(userId: null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanUpload_WhenAuthorizationEnforced_WithoutUser_ShouldReturnFalse()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new FileSecurityOptions
        {
            EnforceAuthorization = true
        });
        var service = new FileAuthorizationService(options);

        // Act
        var result = await service.CanUploadAsync(userId: null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanUpload_WhenAuthorizationEnforced_WithUser_ShouldReturnTrue()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new FileSecurityOptions
        {
            EnforceAuthorization = true
        });
        var service = new FileAuthorizationService(options);

        // Act
        var result = await service.CanUploadAsync(userId: Guid.NewGuid());

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanRead_WhenAuthorizationEnforced_AndUserMatchesOwner_ShouldReturnTrue()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var file = CreateTestFile(ownerId);
        var options = Microsoft.Extensions.Options.Options.Create(new FileSecurityOptions
        {
            EnforceAuthorization = true,
            AllowAnonymousRead = false
        });
        var service = new FileAuthorizationService(options);

        // Act
        var result = await service.CanReadAsync(file, ownerId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanRead_WhenAuthorizationEnforced_AndUserDoesNotMatchOwner_ShouldReturnFalse()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var anotherUser = Guid.NewGuid();
        var file = CreateTestFile(ownerId);
        var options = Microsoft.Extensions.Options.Options.Create(new FileSecurityOptions
        {
            EnforceAuthorization = true,
            AllowAnonymousRead = false
        });
        var service = new FileAuthorizationService(options);

        // Act
        var result = await service.CanReadAsync(file, anotherUser);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanDelete_WhenAuthorizationEnforced_AndUserMatchesOwner_ShouldReturnTrue()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var file = CreateTestFile(ownerId);
        var options = Microsoft.Extensions.Options.Options.Create(new FileSecurityOptions
        {
            EnforceAuthorization = true
        });
        var service = new FileAuthorizationService(options);

        // Act
        var result = await service.CanDeleteAsync(file, ownerId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanDelete_WhenAuthorizationEnforced_AndUserDoesNotMatchOwner_ShouldReturnFalse()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var attackerId = Guid.NewGuid();
        var file = CreateTestFile(ownerId);
        var options = Microsoft.Extensions.Options.Options.Create(new FileSecurityOptions
        {
            EnforceAuthorization = true
        });
        var service = new FileAuthorizationService(options);

        // Act
        var result = await service.CanDeleteAsync(file, attackerId);

        // Assert
        Assert.False(result);
    }
}
