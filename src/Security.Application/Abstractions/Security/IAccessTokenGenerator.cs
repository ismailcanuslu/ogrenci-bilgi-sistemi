namespace Security.Application.Abstractions.Security;

public interface IAccessTokenGenerator
{
    AccessToken Generate(
        Guid userId,
        string email);
}

public sealed record AccessToken(
    string Token,
    DateTime ExpiresAtUtc);