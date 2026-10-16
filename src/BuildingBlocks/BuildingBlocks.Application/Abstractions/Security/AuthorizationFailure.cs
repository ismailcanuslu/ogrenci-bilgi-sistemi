using BuildingBlocks.Application.Results;

namespace BuildingBlocks.Application.Abstractions.Security;

public static class AuthorizationFailure
{
    /// <summary><see cref="IPermissionGuard"/> hatasını uygun HTTP semantiğine sahip sonuca çevirir.</summary>
    public static ServiceResult<T> To<T>(Error error)
        => error.Type == ErrorType.Unauthorized
            ? ServiceResult<T>.Unauthorized(error)
            : ServiceResult<T>.Forbidden(error);
}
