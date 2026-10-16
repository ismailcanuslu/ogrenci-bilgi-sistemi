using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Domain.Authorization;
using MediatR;
using Security.Application.Abstractions.Persistence;

namespace Security.Application.Features.PersonalAccessTokens;

public sealed record PatResponse(
    Guid Id,
    string Name,
    string MaskedToken,
    IReadOnlyCollection<string> Permissions,
    ScopeType ScopeType,
    Guid? ScopeId,
    Guid CreatedByUserId,
    DateTime CreatedAtUtc,
    DateTime? ExpiresAtUtc,
    DateTime? LastUsedAtUtc,
    DateTime? RevokedAtUtc,
    bool IsActive);

public sealed record ListPatsQuery(
    bool? ActiveOnly = true,
    int Page = 1,
    int PageSize = 20) : IRequest<ServiceResult<IReadOnlyCollection<PatResponse>>>;

public sealed class ListPatsHandler(
    IPersonalAccessTokenRepository patRepository,
    IPermissionGuard guard) : IRequestHandler<ListPatsQuery, ServiceResult<IReadOnlyCollection<PatResponse>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<PatResponse>>> Handle(
        ListPatsQuery request, CancellationToken cancellationToken)
    {
        var denied = await guard.CheckAsync(
            BuildingBlocks.Application.Abstractions.Security.Permissions.Authorization.PatRead,
            [ResourceScope.University],
            cancellationToken);

        if (denied is not null)
        {
            return AuthorizationFailure.To<IReadOnlyCollection<PatResponse>>(denied);
        }

        var nowUtc = DateTime.UtcNow;
        var items = await patRepository.ListAsync(
            request.ActiveOnly ?? false, request.Page, request.PageSize, cancellationToken);

        var dtos = items.Select(x => new PatResponse(
            x.Id,
            x.Name,
            x.MaskedToken,
            x.Permissions.Select(p => p.Permission).ToArray(),
            x.ScopeType,
            x.ScopeId,
            x.CreatedByUserId,
            x.CreatedAtUtc,
            x.ExpiresAtUtc,
            x.LastUsedAtUtc,
            x.RevokedAtUtc,
            x.IsActive(nowUtc))).ToList();

        return ServiceResult<IReadOnlyCollection<PatResponse>>.Ok(dtos);
    }
}
