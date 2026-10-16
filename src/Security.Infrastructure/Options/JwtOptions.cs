namespace Security.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = null!;

    public string Audience { get; init; } = null!;

    public int AccessTokenLifetimeMinutes { get; init; } = 15;

    public string PrivateKey { get; init; } = null!;
    
    public string PublicKey { get; init; } = null!;
}