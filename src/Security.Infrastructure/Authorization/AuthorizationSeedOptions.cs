namespace Security.Infrastructure.Authorization;

public sealed class AuthorizationSeedOptions
{
    public const string SectionName = "Authorization:Seed";

    /// <summary>
    /// Doluysa ve bu e-postaya sahip kullanıcı varsa, kullanıcıya üniversite genelinde
    /// System Administrator rolü (yoksa) atanır. İlk yönetici için bootstrap mekanizmasıdır.
    /// </summary>
    public string? BootstrapSystemAdministratorEmail { get; init; }
}
