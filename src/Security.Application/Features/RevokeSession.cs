using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;

namespace Security.Application.Features;

public static class RevokeSession
{
    public sealed record Command(string RefreshToken)
        : ICommand<ServiceResult>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty();
        }
    }

    public sealed class Handler(
        ISessionRepository sessionRepository,
        ITokenHasher tokenHasher,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult>
    {
        public async Task<ServiceResult> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var tokenHash = tokenHasher.Hash(request.RefreshToken);

            var session = await sessionRepository.GetByRefreshTokenHashAsync(
                tokenHash,
                cancellationToken);

            if (session is not null && !session.IsRevoked)
            {
                session.Revoke(DateTime.UtcNow);
                sessionRepository.Update(session);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return ServiceResult.NoContent();
        }
    }
}
