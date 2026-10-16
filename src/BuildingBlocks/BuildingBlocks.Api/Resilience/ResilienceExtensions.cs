using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace BuildingBlocks.Api.Resilience;

public static class ResilienceExtensions
{
    public static IHttpClientBuilder AddBuildingBlocksResilience(
        this IHttpClientBuilder builder,
        IConfiguration? configuration = null)
    {
        var options = new ResilienceOptions();
        configuration?.GetSection(ResilienceOptions.SectionName).Bind(options);

        builder.AddStandardResilienceHandler(resilience =>
        {
            resilience.TotalRequestTimeout.Timeout = options.TotalRequestTimeout;

            resilience.AttemptTimeout.Timeout = options.AttemptTimeout;

            resilience.Retry.MaxRetryAttempts = options.MaxRetryAttempts;
            resilience.Retry.Delay = options.BaseDelay;
            resilience.Retry.BackoffType = DelayBackoffType.Exponential;
            resilience.Retry.UseJitter = true;

            resilience.CircuitBreaker.FailureRatio = options.CircuitBreakerFailureRatio;
            resilience.CircuitBreaker.SamplingDuration = options.CircuitBreakerSamplingDuration;
            resilience.CircuitBreaker.BreakDuration = options.CircuitBreakerBreakDuration;
            resilience.CircuitBreaker.MinimumThroughput = options.CircuitBreakerMinimumThroughput;
        });

        return builder;
    }
}
