namespace Security.Application.Abstractions.Security;

public interface ITokenHasher
{
    byte[] Hash(string token);

    bool Verify(
        string token,
        byte[] tokenHash);
}