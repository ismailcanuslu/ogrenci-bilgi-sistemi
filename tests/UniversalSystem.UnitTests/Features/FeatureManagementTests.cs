using BuildingBlocks.Api.Features;
using BuildingBlocks.Application.Abstractions.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace UniversalSystem.UnitTests.Features;

public class FeatureManagementTests
{
    private IServiceProvider CreateServiceProvider(Dictionary<string, string?> configValues)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddBuildingBlocksFeatureManagement(configuration);

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task IsEnabledAsync_ShouldReturnTrue_WhenFeatureIsEnabledInConfiguration()
    {
        // Arrange
        var sp = CreateServiceProvider(new Dictionary<string, string?>
        {
            { "FeatureManagement:NewDashboard", "true" },
            { "FeatureManagement:BetaFeature", "false" }
        });
        var featureService = sp.GetRequiredService<IFeatureService>();

        // Act
        var isNewDashboardEnabled = await featureService.IsEnabledAsync("NewDashboard");
        var isBetaEnabled = await featureService.IsEnabledAsync("BetaFeature");
        var isMissingEnabled = await featureService.IsEnabledAsync("NonExistent");

        // Assert
        Assert.True(isNewDashboardEnabled);
        Assert.False(isBetaEnabled);
        Assert.False(isMissingEnabled);
    }

    [Fact]
    public async Task GetAllFeaturesAsync_ShouldReturnAllConfiguredFeatures()
    {
        // Arrange
        var sp = CreateServiceProvider(new Dictionary<string, string?>
        {
            { "FeatureManagement:FeatureA", "true" },
            { "FeatureManagement:FeatureB", "false" }
        });
        var featureService = sp.GetRequiredService<IFeatureService>();

        // Act
        var all = await featureService.GetAllFeaturesAsync();

        // Assert
        Assert.Contains("FeatureA", all.Keys);
        Assert.True(all["FeatureA"]);
        Assert.Contains("FeatureB", all.Keys);
        Assert.False(all["FeatureB"]);
    }

    [Fact]
    public async Task RequireFeatureEndpointFilter_ShouldBlock_WhenFeatureDisabled()
    {
        // Arrange
        var sp = CreateServiceProvider(new Dictionary<string, string?>
        {
            { "FeatureManagement:SecretV2", "false" }
        });
        var filter = new RequireFeatureEndpointFilter("SecretV2");

        var httpContext = new DefaultHttpContext { RequestServices = sp };
        var invocationContext = new DefaultEndpointFilterInvocationContext(httpContext);

        var wasNextCalled = false;
        EndpointFilterDelegate next = ctx =>
        {
            wasNextCalled = true;
            return ValueTask.FromResult<object?>("OK");
        };

        // Act
        var result = await filter.InvokeAsync(invocationContext, next);

        // Assert
        Assert.False(wasNextCalled);
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);
    }

    [Fact]
    public async Task RequireFeatureEndpointFilter_ShouldPass_WhenFeatureEnabled()
    {
        // Arrange
        var sp = CreateServiceProvider(new Dictionary<string, string?>
        {
            { "FeatureManagement:SecretV2", "true" }
        });
        var filter = new RequireFeatureEndpointFilter("SecretV2");

        var httpContext = new DefaultHttpContext { RequestServices = sp };
        var invocationContext = new DefaultEndpointFilterInvocationContext(httpContext);

        var wasNextCalled = false;
        EndpointFilterDelegate next = ctx =>
        {
            wasNextCalled = true;
            return ValueTask.FromResult<object?>("Success");
        };

        // Act
        var result = await filter.InvokeAsync(invocationContext, next);

        // Assert
        Assert.True(wasNextCalled);
        Assert.Equal("Success", result);
    }
}
