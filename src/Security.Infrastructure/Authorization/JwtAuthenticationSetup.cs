using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Security.Infrastructure.Options;

namespace Security.Infrastructure.Authorization;

/// <summary>
/// AccessTokenGenerator'ın ürettiği RS256 JWT'lerini doğrular. Algoritma sabit kodlanmıştır;
/// "none" ve simetrik algoritmalar reddedilir.
/// </summary>
internal sealed class JwtAuthenticationSetup(IOptions<JwtOptions> jwtOptions)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(JwtBearerOptions options) => Configure(JwtBearerDefaults.AuthenticationScheme, options);

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        var jwt = jwtOptions.Value;

        // Doğrulama için yalnızca genel anahtar kullanılır; özel anahtar PEM'den türetilir.
        var rsa = RSA.Create();
        rsa.ImportFromPem(jwt.PrivateKey);
        var publicParameters = rsa.ExportParameters(includePrivateParameters: false);
        rsa.Dispose();

        var publicRsa = RSA.Create();
        publicRsa.ImportParameters(publicParameters);

        options.MapInboundClaims = false;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicRsa),
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "sub"
        };
    }
}
