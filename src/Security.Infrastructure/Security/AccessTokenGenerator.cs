using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Security.Application.Abstractions.Security;
using Security.Infrastructure.Options;

namespace Security.Infrastructure.Security;

public sealed class AccessTokenGenerator : IAccessTokenGenerator, IDisposable
{
    private readonly JwtOptions _options;
    private readonly RSA _rsa;

    public AccessTokenGenerator(
        IOptions<JwtOptions> options)
    {
        _options = options.Value;

        _rsa = RSA.Create();
        _rsa.ImportFromPem(_options.PrivateKey);
    }

    public AccessToken Generate(
        Guid userId,
        string email)
    {
        var now = DateTime.UtcNow;

        var expiresAt = now.AddMinutes(
            _options.AccessTokenLifetimeMinutes);

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userId.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                email),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var signingKey = new RsaSecurityKey(_rsa);

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenValue =
            new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessToken(
            tokenValue,
            expiresAt);
    }

    public void Dispose()
    {
        _rsa.Dispose();
    }
}