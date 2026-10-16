namespace Security.Application.Abstractions.Security;

public sealed record PasskeyAuthenticationRequest(
    string CredentialId,
    string ClientDataJson,
    string AuthenticatorData,
    string Signature,
    string? UserHandle);