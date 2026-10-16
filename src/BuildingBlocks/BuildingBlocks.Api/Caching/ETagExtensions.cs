using System.Security.Cryptography;
using System.Text;

namespace BuildingBlocks.Api.Caching;

public static class ETagExtensions
{
    public static string ToETag(this string value)
    {
        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(value));

        return $"\"{Convert.ToHexString(hash)}\"";
    }
}