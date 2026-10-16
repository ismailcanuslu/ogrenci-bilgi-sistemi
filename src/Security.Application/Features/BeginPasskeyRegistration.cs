using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Domain.Challenges;

namespace Security.Application.Features;

public static class BeginPasskeyRegistration
{
    public sealed record Command(
        Guid UserId)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        Guid ChallengeId,
        string OptionsJson);

    public sealed class Handler(
        IUserRepository userRepository,
        IPasskeyService passkeyService,
        IPasskeyRegistrationChallengeRepository challengeRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult<Response>>
    {
        private static readonly TimeSpan ChallengeLifetime =
            TimeSpan.FromMinutes(5);

        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(
                request.UserId,
                cancellationToken);

            if (user is null)
            {
                return ServiceResult<Response>.NotFound(
                    new Error(
                        "security.user.not_found",
                        ErrorType.NotFound));
            }

            if (!user.IsActive)
            {
                return ServiceResult<Response>.Forbidden(
                    new Error(
                        "security.user.inactive",
                        ErrorType.Forbidden));
            }

            var optionsJson = passkeyService.CreateRegistrationOptions(
                user.Id,
                user.Email.Value,
                user.PasskeyCredentials);

            var nowUtc = DateTimeOffset.UtcNow;
            var expiresAtUtc = nowUtc.Add(ChallengeLifetime);

            var challenge = PasskeyRegistrationChallenge.Create(
                user.Id,
                optionsJson,
                nowUtc,
                expiresAtUtc);

            await challengeRepository.AddAsync(
                challenge,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            return ServiceResult<Response>.Ok(
                new Response(
                    challenge.Id,
                    optionsJson));
        }
    }
}