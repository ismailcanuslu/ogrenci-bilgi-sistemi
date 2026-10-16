using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BuildingBlocks.Api.Observability;

public static class UniversalTelemetry
{
    public const string ServicePrefix = "OBS";
    public const string PlatformActivitySourceName = "OBS.Platform";

    public static readonly ActivitySource PlatformActivitySource = new(PlatformActivitySourceName, "1.0.0");
    public static readonly Meter PlatformMeter = new("OBS.Platform", "1.0.0");

    public static ActivitySource CreateSource(string moduleName) => new($"OBS.{moduleName}", "1.0.0");
}
