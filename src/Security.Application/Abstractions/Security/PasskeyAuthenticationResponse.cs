namespace Security.Application.Abstractions.Security;

public sealed record PasskeyAuthenticationResponse(
    bool IsSuccessful,
    uint SignCount);