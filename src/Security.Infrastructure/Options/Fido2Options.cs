namespace Security.Infrastructure.Options;

public sealed class Fido2Options
{
    public const string SectionName = "Fido2";

    public string Rpid { get; init; } = null!;

    public string RpName { get; init; } = null!;

    public string Origin { get; init; } = null!;
}