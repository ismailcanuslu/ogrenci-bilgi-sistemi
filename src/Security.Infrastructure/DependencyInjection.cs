using BuildingBlocks.Application.Abstractions.Persistence;
using Fido2NetLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Abstractions.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Security.Application.Abstractions.Persistence;
using Security.Infrastructure.Authorization;
using Security.Application.Abstractions.Security;
using Security.Infrastructure.Options;
using Security.Infrastructure.Persistence;
using Security.Infrastructure.Persistence.Repositories;
using Security.Infrastructure.Security;

namespace Security.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSecurityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<SecurityDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
        
        services
            .AddOptions<Fido2Options>()
            .Bind(configuration.GetSection(Fido2Options.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Rpid),
                "FATAL: FIDO2 RP ID doldurulmamış.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.RpName),
                "FATAL: FIDO2 RP name doldurulmamış.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Origin),
                "FATAL: FIDO2 origin doldurulmamış.")
            .ValidateOnStart();

        services.AddSingleton<IFido2>(provider =>
        {
            var options = provider
                .GetRequiredService<
                    Microsoft.Extensions.Options.IOptions<Fido2Options>>()
                .Value;

            return new Fido2(new Fido2Configuration
            {
                RPID = options.Rpid,
                RPName = options.RpName,
                Origins = new HashSet<string>
                {
                    options.Origin
                }
            });
        });
        
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Issuer),
                "FATAL: JWT issuer bilgisi doldurulmamış.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Audience),
                "FATAL: JWT audience bilgisi doldurulmamış.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.PrivateKey),
                "FATAL: JWT private key doldurulmamış.")
            .Validate(
                options => options.AccessTokenLifetimeMinutes > 0,
                "FATAL: JWT access token lifetime sıfırdan büyük olmalıdır.")
            .ValidateOnStart();

        services.AddHttpContextAccessor();

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = "SmartScheme";
                options.DefaultAuthenticateScheme = "SmartScheme";
                options.DefaultChallengeScheme = "SmartScheme";
            })
            .AddPolicyScheme("SmartScheme", "JWT or PAT", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    if (context.Request.Headers.ContainsKey(PatAuthenticationDefaults.HeaderName) ||
                        context.Request.Headers.ContainsKey(PatAuthenticationDefaults.AltHeaderName) ||
                        context.Request.Headers["Authorization"].ToString().StartsWith("Bearer " + PatAuthenticationDefaults.TokenPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        return PatAuthenticationDefaults.AuthenticationScheme;
                    }

                    return JwtBearerDefaults.AuthenticationScheme;
                };
            })
            .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, PatAuthenticationHandler>(
                PatAuthenticationDefaults.AuthenticationScheme, null)
            .AddJwtBearer();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtAuthenticationSetup>();
        services.AddAuthorization();

        services
            .AddOptions<AuthorizationSeedOptions>()
            .Bind(configuration.GetSection(AuthorizationSeedOptions.SectionName));
        services.AddHostedService<AuthorizationSeeder>();

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IPermissionEvaluator, PermissionEvaluator>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleAssignmentRepository, UserRoleAssignmentRepository>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IPersonalAccessTokenRepository, PersonalAccessTokenRepository>();

        services.AddScoped<ISecurityUnitOfWork>(
            provider => provider.GetRequiredService<SecurityDbContext>());

        services.AddScoped<IUnitOfWork>(
            provider => provider.GetRequiredService<SecurityDbContext>());

        services.AddScoped<
            IPasskeyAuthenticationChallengeRepository,
            PasskeyAuthenticationChallengeRepository>();

        services.AddScoped<
            IPasskeyRegistrationChallengeRepository,
            PasskeyRegistrationChallengeRepository>();

        services.AddScoped<IPasskeyService, Fido2PasskeyService>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<IAccessTokenGenerator, AccessTokenGenerator>();
        services.AddSingleton<ITokenHasher, TokenHasher>();

        return services;
    }
}