using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Domain.Entities;
using Security.Domain.ValueObjects;

namespace Security.Application.Features;

public static class LoginWithPassword
{
    private const string InvalidCredentialsCode =
        "security.authentication.invalid_credentials";

    private const int MaxFailedLoginAttempts = 5;

    private static readonly TimeSpan LockoutDuration =
        TimeSpan.FromMinutes(15);

    private static readonly TimeSpan SessionLifetime =
        TimeSpan.FromDays(30);

    public sealed record Command(
        string Email,
        string Password)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAtUtc,
        IReadOnlyList<string> Roles);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }

    public sealed class Handler(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IPasswordHasher passwordHasher,
        ITokenHasher tokenHasher,
        IRefreshTokenGenerator refreshTokenGenerator,
        IAccessTokenGenerator accessTokenGenerator,
        IUserRoleAssignmentRepository userRoleAssignmentRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<
            Command,
            ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);

            var user = await userRepository.GetByEmailAsync(
                email,
                cancellationToken);

            if (user is null ||
                user.PasswordCredential is null ||
                !user.IsActive)
            {
                return Unauthorized();
            }

            var nowUtc = DateTime.UtcNow;

            if (user.IsLocked(nowUtc))
            {
                return Unauthorized();
            }

            var passwordValid = passwordHasher.Verify(
                request.Password,
                user.PasswordCredential.PasswordHash);

            if (!passwordValid)
            {
                user.RecordFailedLogin(
                    nowUtc,
                    MaxFailedLoginAttempts,
                    LockoutDuration);

                await unitOfWork.SaveChangesAsync(
                    cancellationToken);

                return Unauthorized();
            }

            user.RecordSuccessfulLogin(nowUtc);

            user.PasswordCredential.MarkAsUsed(nowUtc);

            var sessionExpiresAtUtc =
                nowUtc.Add(SessionLifetime);

            var session = Session.Create(
                user.Id,
                nowUtc,
                sessionExpiresAtUtc);

            var refreshToken =
                refreshTokenGenerator.Generate();

            var refreshTokenHash =
                tokenHasher.Hash(refreshToken);

            session.AddRefreshToken(
                refreshTokenHash,
                nowUtc,
                sessionExpiresAtUtc);

            await sessionRepository.AddAsync(
                session,
                cancellationToken);

            var accessToken = accessTokenGenerator.Generate(
                user.Id,
                user.Email.Value);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            var roles = await userRoleAssignmentRepository.GetRoleNamesByUserIdAsync(
                user.Id,
                cancellationToken);

            return ServiceResult<Response>.Ok(
                new Response(
                    accessToken.Token,
                    refreshToken,
                    accessToken.ExpiresAtUtc,
                    roles));
        }

        private static ServiceResult<Response> Unauthorized()
        {
            return ServiceResult<Response>.Unauthorized(
                new Error(
                    InvalidCredentialsCode,
                    ErrorType.Unauthorized));
        }
    }
}