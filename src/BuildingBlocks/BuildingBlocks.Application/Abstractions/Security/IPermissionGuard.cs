using BuildingBlocks.Application.Results;

namespace BuildingBlocks.Application.Abstractions.Security;

/// <summary>
/// Handler'lar için ortak yetki denetimi. Yetkilendirme mantığı handler'lara kopyalanmaz.
/// </summary>
public interface IPermissionGuard
{
    /// <summary>
    /// İzin ve kapsam denetimi yapar. İzin verilirse null; kimlik doğrulanmamışsa Unauthorized,
    /// yetki yoksa Forbidden hatası döner.
    /// </summary>
    Task<Error?> CheckAsync(
        string permission,
        IReadOnlyCollection<ResourceScope> candidateScopes,
        CancellationToken cancellationToken = default);
}
