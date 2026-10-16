using System.Security.Cryptography;
using System.Text;
using Webhooks.Application.Abstractions;

namespace Webhooks.Infrastructure.Services;

public sealed class HmacWebhookSigner : IWebhookSigner
{
    public string ComputeSignature(string payload, string secret)
    {
        if (payload == null) throw new ArgumentNullException(nameof(payload));
        if (string.IsNullOrEmpty(secret)) throw new ArgumentException("Secret cannot be empty", nameof(secret));

        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(payloadBytes);
        return "sha256=" + Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool VerifySignature(string payload, string secret, string signature)
    {
        if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(signature))
        {
            return false;
        }

        var expectedSignature = ComputeSignature(payload, secret);

        var expectedBytes = Encoding.UTF8.GetBytes(expectedSignature);
        var actualBytes = Encoding.UTF8.GetBytes(signature);

        if (expectedBytes.Length != actualBytes.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
    }
}
