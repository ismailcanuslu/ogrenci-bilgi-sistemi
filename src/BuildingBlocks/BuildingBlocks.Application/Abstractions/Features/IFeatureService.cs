namespace BuildingBlocks.Application.Abstractions.Features;

/// <summary>
/// Proje genelinde dinamik veya konfigürasyon tabanlı feature flag (özellik anahtarı) denetimi sağlar.
/// </summary>
public interface IFeatureService
{
    /// <summary>
    /// Belirtilen özelliğin aktif olup olmadığını doğrular.
    /// </summary>
    Task<bool> IsEnabledAsync(string featureName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tanımlı tüm feature flag'lerin anlık durumunu döndürür.
    /// </summary>
    Task<IReadOnlyDictionary<string, bool>> GetAllFeaturesAsync(CancellationToken cancellationToken = default);
}
