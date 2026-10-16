using System.Security.Cryptography;
using System.Text;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Domain.Authorization;
using FluentValidation;
using MediatR;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Authorization;

namespace Security.Application.Features.PersonalAccessTokens;

public static class PatConstants
{
    public const string TokenPrefix = "pat_live_";

    public static string ComputeSha256(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

public sealed record CreatePatResponse(
    Guid Id,
    string RawToken,
    string MaskedToken,
    IReadOnlyCollection<string> Permissions,
    ScopeType ScopeType,
    Guid? ScopeId,
    DateTime? ExpiresAtUtc);

public sealed record CreatePatCommand(
    string Name,
    IReadOnlyCollection<string> Permissions,
    ScopeType ScopeType = ScopeType.University,
    Guid? ScopeId = null,
    DateTime? ExpiresAtUtc = null) : IRequest<ServiceResult<CreatePatResponse>>;

public sealed class CreatePatValidator : AbstractValidator<CreatePatCommand>
{
    public CreatePatValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Pat.NameRequired")
            .MaximumLength(PersonalAccessToken.NameMaxLength).WithMessage("Pat.NameTooLong");

        RuleFor(x => x.Permissions)
            .NotEmpty().WithMessage("Pat.AtLeastOnePermissionRequired");

        RuleForEach(x => x.Permissions)
            .Must(p => BuildingBlocks.Application.Abstractions.Security.Permissions.All.Contains(p.Trim().ToLowerInvariant()))
            .WithMessage("Pat.InvalidPermissionInList");

        When(x => x.ScopeType != ScopeType.University, () =>
        {
            RuleFor(x => x.ScopeId).NotEmpty().WithMessage("Pat.ScopeIdRequiredForNarrowScope");
        });

        When(x => x.ExpiresAtUtc.HasValue, () =>
        {
            RuleFor(x => x.ExpiresAtUtc!.Value)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Pat.ExpirationMustBeInFuture");
        });
    }
}

public sealed class CreatePatHandler(
    IPersonalAccessTokenRepository patRepository,
    ISecurityUnitOfWork unitOfWork,
    IPermissionGuard guard,
    ICurrentUser currentUser,
    IAuditService auditService) : IRequestHandler<CreatePatCommand, ServiceResult<CreatePatResponse>>
{
    public async Task<ServiceResult<CreatePatResponse>> Handle(
        CreatePatCommand request, CancellationToken cancellationToken)
    {
        var denied = await guard.CheckAsync(
            BuildingBlocks.Application.Abstractions.Security.Permissions.Authorization.PatCreate,
            [ResourceScope.University],
            cancellationToken);

        if (denied is not null)
        {
            return AuthorizationFailure.To<CreatePatResponse>(denied);
        }

        var creatorId = currentUser.UserId ?? Guid.Empty;

        // Kriptografik rastgele token üretimi (32 bayt = 256 bit entropi)
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        var base64 = Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

        var rawToken = $"{PatConstants.TokenPrefix}{base64}";
        var tokenHash = PatConstants.ComputeSha256(rawToken);

        var maskedToken = $"{PatConstants.TokenPrefix}{base64[..6]}...{base64[^4..]}";
        var nowUtc = DateTime.UtcNow;

        var pat = PersonalAccessToken.Create(
            request.Name,
            tokenHash,
            maskedToken,
            request.Permissions,
            request.ScopeType,
            request.ScopeId,
            creatorId,
            request.ExpiresAtUtc,
            nowUtc);

        await patRepository.AddAsync(pat, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Pat.Created",
            "Security",
            nameof(PersonalAccessToken),
            pat.Id.ToString(),
            newValues: new
            {
                pat.Name,
                pat.MaskedToken,
                Permissions = request.Permissions,
                pat.ScopeType,
                pat.ScopeId,
                pat.ExpiresAtUtc
            },
            cancellationToken: cancellationToken);

        return ServiceResult<CreatePatResponse>.Created(new CreatePatResponse(
            pat.Id,
            rawToken,
            maskedToken,
            pat.Permissions.Select(p => p.Permission).ToArray(),
            pat.ScopeType,
            pat.ScopeId,
            pat.ExpiresAtUtc));
    }
}
