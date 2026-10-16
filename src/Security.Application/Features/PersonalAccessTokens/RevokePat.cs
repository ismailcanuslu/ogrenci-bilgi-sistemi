using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Authorization;

namespace Security.Application.Features.PersonalAccessTokens;

public sealed record RevokePatCommand(Guid Id) : IRequest<ServiceResult<Unit>>;

public sealed class RevokePatValidator : AbstractValidator<RevokePatCommand>
{
    public RevokePatValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Pat.IdRequired");
    }
}

public sealed class RevokePatHandler(
    IPersonalAccessTokenRepository patRepository,
    ISecurityUnitOfWork unitOfWork,
    IPermissionGuard guard,
    ICurrentUser currentUser,
    IAuditService auditService) : IRequestHandler<RevokePatCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(RevokePatCommand request, CancellationToken cancellationToken)
    {
        var denied = await guard.CheckAsync(
            BuildingBlocks.Application.Abstractions.Security.Permissions.Authorization.PatRevoke,
            [ResourceScope.University],
            cancellationToken);

        if (denied is not null)
        {
            return AuthorizationFailure.To<Unit>(denied);
        }

        var pat = await patRepository.GetByIdAsync(request.Id, cancellationToken);
        if (pat is null)
        {
            return ServiceResult<Unit>.NotFound(new Error("Pat.NotFound", ErrorType.NotFound));
        }

        var revokerId = currentUser.UserId ?? Guid.Empty;
        pat.Revoke(revokerId, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Pat.Revoked",
            "Security",
            nameof(PersonalAccessToken),
            pat.Id.ToString(),
            newValues: new { pat.Name, pat.MaskedToken, pat.RevokedAtUtc },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
