using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Domain.Enums;

namespace Security.Application.Features;

public static class CompletePasskeyRegistration
{
    private const string InvalidRegistrationCode =
        "security.passkey_registration.invalid_registration";

    public sealed record Command(
        Guid ChallengeId,
        PasskeyRegistrationRequest Registration)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        Guid CredentialId);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ChallengeId)
                .NotEmpty();

            RuleFor(x => x.Registration)
                .NotNull();

            RuleFor(x => x.Registration.CredentialId)
                .NotEmpty();

            RuleFor(x => x.Registration.ClientDataJson)
                .NotEmpty();

            RuleFor(x => x.Registration.AttestationObject)
                .NotEmpty();
        }
    }

    public sealed class Handler(
        IPasskeyRegistrationChallengeRepository challengeRepository,
        IUserRepository userRepository,
        IPasskeyService passkeyService,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult<Response>>
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
            {
                return ServiceResult<Response>.NotFound(
                    new Error(
                        "security.passkey_registration_challenge.not_found",
                        ErrorType.NotFound));
            }

            var nowUtc = DateTimeOffset.UtcNow;

            if (challenge.IsUsed || challenge.IsExpired(nowUtc))
            {
                return ServiceResult<Response>.Unauthorized(
                    new Error(
                        InvalidRegistrationCode,
                        ErrorType.Unauthorized));
            }

            var user =
                await userRepository.GetByIdAsync(
                    challenge.UserId,
                    cancellationToken);

            if (user is null || !user.IsActive)
            {
                return ServiceResult<Response>.Unauthorized(
                    new Error(
                        InvalidRegistrationCode,
                        ErrorType.Unauthorized));
            }

            PasskeyRegistrationResponse registration;

            try
            {
                registration =
                    await passkeyService.RegisterAsync(
                        challenge.OptionsJson,
                        request.Registration,
                        user.Id,
                        cancellationToken);
            }
            catch
            {
                return ServiceResult<Response>.Unauthorized(
                    new Error(
                        InvalidRegistrationCode,
                        ErrorType.Unauthorized));
            }

            var credential = user.AddPasskeyCredential(
                registration.CredentialId,
                registration.PublicKey,
                registration.UserHandle,
                AuthenticatorType.Platform);

            challenge.MarkAsUsed(nowUtc);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<Response>.Created(
                new Response(credential.Id));
        }
    }
}