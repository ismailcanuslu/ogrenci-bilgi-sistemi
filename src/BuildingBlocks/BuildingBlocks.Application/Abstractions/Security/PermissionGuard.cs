using BuildingBlocks.Application.Results;

namespace BuildingBlocks.Application.Abstractions.Security;

public sealed class PermissionGuard(
    ICurrentUser currentUser,
    IPermissionEvaluator evaluator)
    : IPermissionGuard
{
    public const string UnauthenticatedCode = "Authorization.Unauthenticated";

    public const string ForbiddenCode = "Authorization.Forbidden";

    public async Task<Error?> CheckAsync(
        string permission,
        IReadOnlyCollection<ResourceScope> candidateScopes,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return new Error(UnauthenticatedCode, ErrorType.Unauthorized);
        }

        var allowed = await evaluator.HasPermissionAsync(
            userId,
            permission,
            candidateScopes,
            cancellationToken);

        return allowed
            ? null
            : new Error(ForbiddenCode, ErrorType.Forbidden);
    }
}
