using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;

namespace Security.Application.Features;

public static class RefreshToken
{
    private const string InvalidTokenCode =
        "security.authentication.invalid_refresh_token";

    private static readonly TimeSpan SessionLifetime =
        TimeSpan.FromDays(30);

    public sealed record Command(string Token)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAtUtc);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();
        }
    }

    public sealed class Handler(
        ISessionRepository sessionRepository,
        IUserRepository userRepository,
        ITokenHasher tokenHasher,
        IRefreshTokenGenerator refreshTokenGenerator,
        IAccessTokenGenerator accessTokenGenerator,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var tokenHash = tokenHasher.Hash(request.Token);

            var session = await sessionRepository.GetByRefreshTokenHashAsync(
                tokenHash,
                cancellationToken);

            var nowUtc = DateTime.UtcNow;

            if (session is null ||
                session.IsRevoked ||
                session.ExpiresAtUtc <= nowUtc)
            {
                return Unauthorized();
            }

            var currentRefreshToken = session.RefreshTokens
                .FirstOrDefault(x => x.TokenHash.SequenceEqual(tokenHash));

            if (currentRefreshToken is null ||
                currentRefreshToken.IsUsed ||
                currentRefreshToken.IsRevoked ||
                currentRefreshToken.ExpiresAtUtc <= nowUtc)
            {
                return Unauthorized();
            }

            var user = await userRepository.GetByIdAsync(
                session.UserId,
                cancellationToken);

            if (user is null || !user.IsActive)
            {
                return Unauthorized();
            }

            var newRawRefreshToken = refreshTokenGenerator.Generate();
            var newRefreshTokenHash = tokenHasher.Hash(newRawRefreshToken);

            var replacementExpiresAtUtc = nowUtc.Add(SessionLifetime);

            session.RotateRefreshToken(
                currentRefreshToken.Id,
                tokenHash,
                newRefreshTokenHash,
                nowUtc,
                replacementExpiresAtUtc);

            sessionRepository.Update(session);

            var accessToken = accessTokenGenerator.Generate(
                user.Id,
                user.Email.Value);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<Response>.Ok(
                new Response(
                    accessToken.Token,
                    newRawRefreshToken,
                    accessToken.ExpiresAtUtc));
        }

        private static ServiceResult<Response> Unauthorized()
        {
            return ServiceResult<Response>.Unauthorized(
                new Error(
                    InvalidTokenCode,
                    ErrorType.Unauthorized));
        }
    }
}
