using System.Security.Cryptography;
using Security.Application.Abstractions.Security;

namespace Security.Infrastructure.Security;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private const int TokenSize = 32;

    public string Generate()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(TokenSize);

        return Convert.ToBase64String(tokenBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}