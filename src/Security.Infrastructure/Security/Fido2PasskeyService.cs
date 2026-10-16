using System.Buffers.Text;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Security.Application.Abstractions.Security;
using Security.Domain.Entities;

namespace Security.Infrastructure.Security;

public sealed class Fido2PasskeyService(
    IFido2 fido2)
    : IPasskeyService
{
    public string CreateRegistrationOptions(
        Guid userId,
        string email,
        IReadOnlyCollection<PasskeyCredential> credentials)
    {
        var user = new Fido2User
        {
            Id = userId.ToByteArray(),
            Name = email,
            DisplayName = email
        };

        var existingCredentials = credentials
            .Where(x => !x.IsRevoked)
            .Select(x => new PublicKeyCredentialDescriptor(
                PublicKeyCredentialType.PublicKey,
                x.CredentialId))
            .ToList();

        var options = fido2.RequestNewCredential(
            new RequestNewCredentialParams
            {
                User = user,
                ExcludeCredentials = existingCredentials,
                AuthenticatorSelection = new AuthenticatorSelection
                {
                    UserVerification =
                        UserVerificationRequirement.Preferred
                },
                AttestationPreference =
                    AttestationConveyancePreference.None
            });

        return options.ToJson();
    }

    public async Task<PasskeyRegistrationResponse> RegisterAsync(
        string optionsJson,
        PasskeyRegistrationRequest request,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var options = CredentialCreateOptions.FromJson(optionsJson);

        var attestationResponse = new AuthenticatorAttestationRawResponse
        {
            Id = request.CredentialId,
            RawId = Base64Url.DecodeFromChars(request.CredentialId),
            Response = new AuthenticatorAttestationRawResponse.AttestationResponse
            {
                ClientDataJson = Base64Url.DecodeFromChars(request.ClientDataJson),
                AttestationObject = Base64Url.DecodeFromChars(request.AttestationObject)
            }
        };

        var result = await fido2.MakeNewCredentialAsync(
            new MakeNewCredentialParams
            {
                AttestationResponse = attestationResponse,
                OriginalOptions = options,
                IsCredentialIdUniqueToUserCallback =
                    (_, _) => Task.FromResult(true)
            },
            cancellationToken);

        return new PasskeyRegistrationResponse(
            result.Id,
            result.PublicKey,
            result.User.Id,
            result.SignCount);
    }

    public string CreateAuthenticationOptions(
        IReadOnlyCollection<PasskeyCredential> credentials)
    {
        var allowedCredentials = credentials
            .Where(x => !x.IsRevoked)
            .Select(x => new PublicKeyCredentialDescriptor(
                PublicKeyCredentialType.PublicKey,
                x.CredentialId))
            .ToList();

        var options = fido2.GetAssertionOptions(
            new GetAssertionOptionsParams
            {
                AllowedCredentials = allowedCredentials,
                UserVerification =
                    UserVerificationRequirement.Preferred
            });

        return options.ToJson();
    }

    public async Task<PasskeyAuthenticationResponse> AuthenticateAsync(
        string optionsJson,
        PasskeyAuthenticationRequest request,
        PasskeyCredential credential,
        CancellationToken cancellationToken)
    {
        var options = AssertionOptions.FromJson(
            optionsJson);

        var rawId = Convert.FromBase64String(
            request.CredentialId);

        var clientDataJson = Convert.FromBase64String(
            request.ClientDataJson);

        var authenticatorData = Convert.FromBase64String(
            request.AuthenticatorData);

        var signature = Convert.FromBase64String(
            request.Signature);

        var userHandle = string.IsNullOrWhiteSpace(
            request.UserHandle)
            ? null
            : Convert.FromBase64String(
                request.UserHandle);

        var assertionResponse =
            new AuthenticatorAssertionRawResponse
            {
                Id = request.CredentialId,
                RawId = rawId,
                Response =
                    new AuthenticatorAssertionRawResponse
                        .AssertionResponse
                    {
                        ClientDataJson = clientDataJson,
                        AuthenticatorData = authenticatorData,
                        Signature = signature,
                        UserHandle = userHandle
                    }
            };

        var result = await fido2.MakeAssertionAsync(
            new MakeAssertionParams
            {
                AssertionResponse = assertionResponse,
                OriginalOptions = options,
                StoredPublicKey = credential.PublicKey,
                StoredSignatureCounter = credential.SignCount,
                IsUserHandleOwnerOfCredentialIdCallback =
                    (args, _) =>
                    {
                        var expectedUserHandle =
                            credential.UserHandle;

                        return Task.FromResult(
                            expectedUserHandle.AsSpan()
                                .SequenceEqual(args.UserHandle));
                    }
            },
            cancellationToken);

        return new PasskeyAuthenticationResponse(
            true,
            result.SignCount);
    }
}