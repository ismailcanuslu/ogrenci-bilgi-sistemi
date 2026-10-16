using BuildingBlocks.Application.Abstractions.Features;
using Microsoft.FeatureManagement;

namespace BuildingBlocks.Api.Features;

public sealed class FeatureService : IFeatureService
{
    private readonly IFeatureManager _featureManager;

    public FeatureService(IFeatureManager featureManager)
    {
        _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
    }

    public async Task<bool> IsEnabledAsync(string featureName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(featureName))
        {
            return false;
        }

        return await _featureManager.IsEnabledAsync(featureName).ConfigureAwait(false);
    }

    public async Task<IReadOnlyDictionary<string, bool>> GetAllFeaturesAsync(CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        await foreach (var name in _featureManager.GetFeatureNamesAsync().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            var isEnabled = await _featureManager.IsEnabledAsync(name).ConfigureAwait(false);
            result[name] = isEnabled;
        }

        return result;
    }
}
