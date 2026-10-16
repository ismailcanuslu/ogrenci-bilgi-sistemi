namespace Security.Application.Abstractions.Security;

public sealed record PasskeyRegistrationRequest(
    string CredentialId,
    string ClientDataJson,
    string AttestationObject,
    string? Transports);