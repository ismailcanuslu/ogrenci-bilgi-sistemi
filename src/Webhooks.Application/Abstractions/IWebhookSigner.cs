namespace Webhooks.Application.Abstractions;

public interface IWebhookSigner
{
    /// <summary>
    /// Verilen payload metni ve secret kullanılarak HMAC-SHA256 imzası hesaplar (hex encoded).
    /// </summary>
    string ComputeSignature(string payload, string secret);

    /// <summary>
    /// Gelen webhook imzasını zamanlama saldırılarına karşı güvenli (CryptographicOperations.FixedTimeEquals) şekilde doğrular.
    /// </summary>
    bool VerifySignature(string payload, string secret, string signature);
}
