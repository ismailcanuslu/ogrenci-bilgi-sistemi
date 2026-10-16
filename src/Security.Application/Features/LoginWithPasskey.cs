using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Domain.Entities;
using Security.Domain.Enums;

namespace Security.Application.Features;

public static class LoginWithPasskey
{
    private const string InvalidCredentialsCode =
        "security.authentication.invalid_credentials";

    private static readonly TimeSpan SessionLifetime =
        TimeSpan.FromDays(30);

    public sealed record Command(
        Guid ChallengeId,
        PasskeyAuthenticationRequest Authentication)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAtUtc);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ChallengeId)
                .NotEmpty();

            RuleFor(x => x.Authentication)
                .NotNull();

            RuleFor(x => x.Authentication.CredentialId)
                .NotEmpty();

            RuleFor(x => x.Authentication.ClientDataJson)
                .NotEmpty();

            RuleFor(x => x.Authentication.AuthenticatorData)
                .NotEmpty();

            RuleFor(x => x.Authentication.Signature)
                .NotEmpty();
        }
    }

    public sealed class Handler(
        IPasskeyAuthenticationChallengeRepository challengeRepository,
        IUserRepository userRepository,
        IPasskeyService passkeyService,
        ISessionRepository sessionRepository,
        ITokenHasher tokenHasher,
        IRefreshTokenGenerator refreshTokenGenerator,
        IAccessTokenGenerator accessTokenGenerator,
        IUnitOfWork unitOfWork)
        : ICommandHandler<
            Command,
            ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var challenge =
                await challengeRepository.GetByIdAsync(
                    request.ChallengeId,
                    cancellationToken);

            if (challenge is null)
                return Unauthorized();

            var nowUtc = DateTimeOffset.UtcNow;

            if (challenge.IsUsed ||
                challenge.IsExpired(nowUtc))
            {
                return Unauthorized();
            }

            var user = await userRepository.GetByIdAsync(
                challenge.UserId,
                cancellationToken);

            if (user is null ||
                !user.IsActive)
            {
                return Unauthorized();
            }

            var passkeyCredential = user.PasskeyCredentials
                .FirstOrDefault(x =>
                    x.CredentialId.AsSpan().SequenceEqual(
                        Convert.FromBase64String(
                            request.Authentication.CredentialId)));

            if (passkeyCredential is null ||
                passkeyCredential.IsRevoked)
            {
                return Unauthorized();
            }

            PasskeyAuthenticationResponse authenticationResult;

            try
            {
                authenticationResult =
                    await passkeyService.AuthenticateAsync(
                        challenge.OptionsJson,
                        request.Authentication,
                        passkeyCredential,
                        cancellationToken);
            }
            catch
            {
                return Unauthorized();
            }

            var passkeyResult =
                passkeyCredential.RecordAuthentication(
                    authenticationResult.SignCount,
                    nowUtc);

            if (passkeyResult ==
                PasskeyAuthenticationResult.SignCountAnomaly)
            {
                return Unauthorized();
            }

            challenge.MarkAsUsed(nowUtc);

            user.RecordSuccessfulLogin(
                nowUtc.UtcDateTime);

            var sessionExpiresAtUtc =
                nowUtc.UtcDateTime.Add(SessionLifetime);

            var session = Session.Create(
                user.Id,
                nowUtc.UtcDateTime,
                sessionExpiresAtUtc);

            var refreshToken =
                refreshTokenGenerator.Generate();

            var refreshTokenHash =
                tokenHasher.Hash(refreshToken);

            session.AddRefreshToken(
                refreshTokenHash,
                nowUtc.UtcDateTime,
                sessionExpiresAtUtc);

            await sessionRepository.AddAsync(
                session,
                cancellationToken);

            var accessToken =
                accessTokenGenerator.Generate(
                    user.Id,
                    user.Email.Value);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            return ServiceResult<Response>.Ok(
                new Response(
                    accessToken.Token,
                    refreshToken,
                    accessToken.ExpiresAtUtc));
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