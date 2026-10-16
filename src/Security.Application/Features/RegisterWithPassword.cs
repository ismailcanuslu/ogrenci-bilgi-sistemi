using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Domain.Entities;
using Security.Domain.ValueObjects;

namespace Security.Application.Features;

public static class RegisterWithPassword
{
    private const string EmailAlreadyRegisteredCode =
        "security.user.email_already_registered";

    public sealed record Command(
        string Email,
        string Password)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        Guid Id,
        string Email,
        DateTime CreatedAtUtc);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);
        }
    }

    public sealed class Handler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);

            var existingUser = await userRepository.GetByEmailAsync(
                email,
                cancellationToken);

            if (existingUser is not null)
            {
                return ServiceResult<Response>.Conflict(
                    new Error(
                        EmailAlreadyRegisteredCode,
                        ErrorType.Conflict));
            }

            var nowUtc = DateTime.UtcNow;
            var user = User.Create(email, nowUtc);

            var passwordHash = passwordHasher.Hash(request.Password);
            user.SetPasswordCredential(passwordHash, nowUtc);

            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<Response>.Created(
                new Response(
                    user.Id,
                    user.Email.Value,
                    user.CreatedAtUtc));
        }
    }
}
