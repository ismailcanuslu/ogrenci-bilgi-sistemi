using System.Text;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using Files.Application.Abstractions.Persistence;
using Files.Application.Abstractions.Security;
using Files.Application.Abstractions.Storage;
using Files.Application.Abstractions.Validation;
using Files.Application.Features;
using Files.Domain.ValueObjects;
using NSubstitute;
using Xunit;
using File = Files.Domain.Files.File;

namespace UniversalSystem.UnitTests.Files;

public class UploadFileHandlerTests
{
    private readonly IFileRepository _fileRepository = Substitute.For<IFileRepository>();
    private readonly IFileStorage _fileStorage = Substitute.For<IFileStorage>();
    private readonly IFileValidator _fileValidator = Substitute.For<IFileValidator>();
    private readonly IFileHashCalculator _hashCalculator = Substitute.For<IFileHashCalculator>();
    private readonly IFileAuthorizationService _authService = Substitute.For<IFileAuthorizationService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUploadAndReturnCreated()
    {
        // Arrange
        var content = "Sample file data";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        _authService.CanUploadAsync(Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _fileValidator.ValidateAsync(
            Arg.Any<Stream>(),
            Arg.Any<FileName>(),
            Arg.Any<ContentType>(),
            Arg.Any<FileSize>(),
            Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var hash = FileHash.Create(new string('f', 64), HashAlgorithm.Sha256).Value!;
        _hashCalculator.ComputeHashAsync(
            Arg.Any<Stream>(),
            Arg.Any<HashAlgorithm>(),
            Arg.Any<CancellationToken>())
            .Returns(hash);

        var storageKey = StorageKey.Create("2026/10/12/saved.txt").Value!;
        _fileStorage.UploadAsync(
            Arg.Any<Stream>(),
            Arg.Any<FileName>(),
            Arg.Any<ContentType>(),
            Arg.Any<CancellationToken>())
            .Returns(storageKey);

        var handler = new UploadFile.Handler(
            _fileRepository,
            _fileStorage,
            _fileValidator,
            _hashCalculator,
            _authService,
            _unitOfWork);

        var command = new UploadFile.Command(
            "report.txt",
            "text/plain",
            stream.Length,
            stream);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("report.txt", result.Data.FileName);
        Assert.Equal(hash.Value, result.Data.Hash);

        await _fileRepository.Received(1).AddAsync(Arg.Any<File>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUnauthorized_ShouldReturnForbidden()
    {
        // Arrange
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Data"));
        _authService.CanUploadAsync(Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var handler = new UploadFile.Handler(
            _fileRepository,
            _fileStorage,
            _fileValidator,
            _hashCalculator,
            _authService,
            _unitOfWork);

        var command = new UploadFile.Command(
            "report.txt",
            "text/plain",
            stream.Length,
            stream);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(403, result.StatusCode);
        Assert.Equal("Files.UnauthorizedUpload", result.Errors.First().Code);
    }
}
