namespace BuildingBlocks.Application.Abstractions.Security;

/// <summary>
/// Permission + kaynak kapsamı denetimini tek noktadan yapan servis.
/// Varsayılan davranış reddetmektir (fail closed).
/// </summary>
public interface IPermissionEvaluator
{
    /// <summary>
    /// Kullanıcı <paramref name="permission"/> iznine, verilen kapsamlardan en az birinde
    /// (veya üniversite genelinde) sahipse true döner. Pasif kullanıcı ve iptal edilmiş
    /// atamalar her zaman yok sayılır.
    /// </summary>
    Task<bool> HasPermissionAsync(
        Guid userId,
        string permission,
        IReadOnlyCollection<ResourceScope> candidateScopes,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Listeleme sorgularını kapsamla sınırlamak için kullanıcının bu izne sahip olduğu
    /// kapsamları döndürür. Boş sonuç = hiçbir kayda erişim yok.
    /// </summary>
    Task<IReadOnlyCollection<ResourceScope>> GetGrantedScopesAsync(
        Guid userId,
        string permission,
        CancellationToken cancellationToken = default);
}
