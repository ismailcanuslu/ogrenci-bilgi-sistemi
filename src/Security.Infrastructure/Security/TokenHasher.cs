using System.Security.Cryptography;
using System.Text;
using Security.Application.Abstractions.Security;

namespace Security.Infrastructure.Security;

public sealed class TokenHasher : ITokenHasher
{
    public byte[] Hash(string token)
    {
        ArgumentException.ThrowIfNullOrEmpty(token);

        return SHA256.HashData(
            Encoding.UTF8.GetBytes(token));
    }

    public bool Verify(
        string token,
        byte[] tokenHash)
    {
        ArgumentException.ThrowIfNullOrEmpty(token);

        if (tokenHash is null ||
            tokenHash.Length != SHA256.HashSizeInBytes)
        {
            return false;
        }

        var actualHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return CryptographicOperations.FixedTimeEquals(
            actualHash,
            tokenHash);
    }
}