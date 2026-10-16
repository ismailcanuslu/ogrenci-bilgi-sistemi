namespace BuildingBlocks.Api.Resilience;

public sealed class ResilienceOptions
{
    public const string SectionName = "Resilience";

    public int MaxRetryAttempts { get; set; } = 3;

    public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(2);

    public TimeSpan AttemptTimeout { get; set; } = TimeSpan.FromSeconds(10);

    public TimeSpan TotalRequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

    public double CircuitBreakerFailureRatio { get; set; } = 0.5;

    public TimeSpan CircuitBreakerSamplingDuration { get; set; } = TimeSpan.FromSeconds(30);

    public TimeSpan CircuitBreakerBreakDuration { get; set; } = TimeSpan.FromSeconds(30);

    public int CircuitBreakerMinimumThroughput { get; set; } = 10;
}
