using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using NSubstitute;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Application.Features;
using Security.Domain.Entities;
using Security.Domain.ValueObjects;
using Xunit;

namespace UniversalSystem.UnitTests.Security;

public class RegisterWithPasswordTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_WhenEmailNotRegistered_ShouldCreateUserAndReturnCreated()
    {
        // Arrange
        _userRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        _passwordHasher.Hash("Password123!")
            .Returns("hashed_password_sample");

        var handler = new RegisterWithPassword.Handler(
            _userRepository,
            _passwordHasher,
            _unitOfWork);

        var command = new RegisterWithPassword.Command(
            "student@obs.edu.tr",
            "Password123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("student@obs.edu.tr", result.Data.Email);

        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyRegistered_ShouldReturnConflict()
    {
        // Arrange
        var existingUser = User.Create(
            Email.Create("student@obs.edu.tr"),
            DateTime.UtcNow);

        _userRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(existingUser);

        var handler = new RegisterWithPassword.Handler(
            _userRepository,
            _passwordHasher,
            _unitOfWork);

        var command = new RegisterWithPassword.Command(
            "student@obs.edu.tr",
            "Password123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(409, result.StatusCode);
        Assert.Contains(result.Errors, e => e.Type == ErrorType.Conflict);

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
