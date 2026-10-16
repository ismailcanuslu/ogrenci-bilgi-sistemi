namespace Security.Application.Abstractions.Security;

public interface IRefreshTokenGenerator
{
    string Generate();
}