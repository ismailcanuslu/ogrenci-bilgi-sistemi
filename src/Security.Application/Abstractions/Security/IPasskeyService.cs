using Security.Domain.Entities;

namespace Security.Application.Abstractions.Security;

public interface IPasskeyService
{
    string CreateAuthenticationOptions(
        IReadOnlyCollection<PasskeyCredential> credentials);

    Task<PasskeyAuthenticationResponse> AuthenticateAsync(
        string optionsJson,
        PasskeyAuthenticationRequest request,
        PasskeyCredential credential,
        CancellationToken cancellationToken);
    
    string CreateRegistrationOptions(
        Guid userId,
        string email,
        IReadOnlyCollection<PasskeyCredential> credentials);

    Task<PasskeyRegistrationResponse> RegisterAsync(
        string optionsJson,
        PasskeyRegistrationRequest request,
        Guid userId,
        CancellationToken cancellationToken);
}