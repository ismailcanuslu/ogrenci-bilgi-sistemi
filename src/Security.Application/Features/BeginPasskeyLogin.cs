using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Domain.Challenges;
using Security.Domain.ValueObjects;

namespace Security.Application.Features;

public static class BeginPasskeyLogin
{
    private static readonly TimeSpan ChallengeLifetime =
        TimeSpan.FromMinutes(5);

    public sealed record Command(string Email)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        Guid ChallengeId,
        string OptionsJson);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }

    public sealed class Handler(
        IUserRepository userRepository,
        IPasskeyService passkeyService,
        IPasskeyAuthenticationChallengeRepository challengeRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);

            var user = await userRepository.GetByEmailAsync(
                email,
                cancellationToken);

            if (user is null)
            {
                return ServiceResult<Response>.Unauthorized(
                    new Error(
                        "security.authentication.invalid_credentials",
                        ErrorType.Unauthorized));
            }

            if (!user.IsActive)
            {
                return ServiceResult<Response>.Unauthorized(
                    new Error(
                        "security.authentication.invalid_credentials",
                        ErrorType.Unauthorized));
            }

            if (user.PasskeyCredentials.Count == 0)
            {
                return ServiceResult<Response>.Unauthorized(
                    new Error(
                        "security.authentication.invalid_credentials",
                        ErrorType.Unauthorized));
            }

            var optionsJson = passkeyService.CreateAuthenticationOptions(
                user.PasskeyCredentials);

            var nowUtc = DateTimeOffset.UtcNow;
            var expiresAtUtc = nowUtc.Add(ChallengeLifetime);

            var challenge = PasskeyAuthenticationChallenge.Create(
                user.Id,
                optionsJson,
                nowUtc,
                expiresAtUtc);

            await challengeRepository.AddAsync(
                challenge,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<Response>.Ok(
                new Response(
                    challenge.Id,
                    optionsJson));
        }
    }
}