namespace Security.Application.Abstractions.Security;

public sealed record PasskeyRegistrationResponse(
    byte[] CredentialId,
    byte[] PublicKey,
    byte[] UserHandle,
    uint SignCount);