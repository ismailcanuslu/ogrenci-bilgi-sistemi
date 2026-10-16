using System.Net;
using BuildingBlocks.Api.Resilience;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace UniversalSystem.UnitTests.Resilience;

public class ResilienceTests
{
    private sealed class TransientErrorDelegatingHandler : DelegatingHandler
    {
        public int AttemptCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AttemptCount++;
            if (AttemptCount < 3)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    [Fact]
    public async Task AddBuildingBlocksResilience_ShouldRetryTransientErrors()
    {
        // Arrange
        var services = new ServiceCollection();
        var handler = new TransientErrorDelegatingHandler();

        services.AddHttpClient("TestResilientClient")
            .ConfigurePrimaryHttpMessageHandler(() => handler)
            .AddBuildingBlocksResilience();

        var serviceProvider = services.BuildServiceProvider();
        var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var client = clientFactory.CreateClient("TestResilientClient");

        // Act
        var response = await client.GetAsync("https://example.com/api/test");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, handler.AttemptCount);
    }
}
