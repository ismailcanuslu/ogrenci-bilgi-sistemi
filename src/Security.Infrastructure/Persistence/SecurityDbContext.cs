using BuildingBlocks.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Security.Domain.Authorization;
using Security.Domain.Challenges;
using Security.Domain.Entities;

namespace Security.Infrastructure.Persistence;

public sealed class SecurityDbContext : DbContext, IUnitOfWork, ISecurityUnitOfWork
{
    public SecurityDbContext(
        DbContextOptions<SecurityDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Session> Sessions => Set<Session>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<PasswordCredential> PasswordCredentials
        => Set<PasswordCredential>();

    public DbSet<PasskeyCredential> PasskeyCredentials
        => Set<PasskeyCredential>();
    
    public DbSet<PasskeyAuthenticationChallenge>
        PasskeyAuthenticationChallenges => Set<PasskeyAuthenticationChallenge>();
    
    public DbSet<PasskeyRegistrationChallenge>
        PasskeyRegistrationChallenges
        => Set<PasskeyRegistrationChallenge>();

    public DbSet<TotpCredential> TotpCredentials
        => Set<TotpCredential>();

    public DbSet<RecoveryCode> RecoveryCodes
        => Set<RecoveryCode>();
    
    public DbSet<EmailVerificationChallenge>
        EmailVerificationChallenges => Set<EmailVerificationChallenge>();

    public DbSet<EmailLoginChallenge>
        EmailLoginChallenges => Set<EmailLoginChallenge>();

    public DbSet<PasswordResetChallenge>
        PasswordResetChallenges => Set<PasswordResetChallenge>();

    public DbSet<EmailMfaChallenge>
        EmailMfaChallenges => Set<EmailMfaChallenge>();

    public DbSet<SmsMfaChallenge>
        SmsMfaChallenges => Set<SmsMfaChallenge>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<UserRoleAssignment> UserRoleAssignments
        => Set<UserRoleAssignment>();

    public DbSet<PersonalAccessToken> PersonalAccessTokens
        => Set<PersonalAccessToken>();

    public DbSet<PersonalAccessTokenPermission> PersonalAccessTokenPermissions
        => Set<PersonalAccessTokenPermission>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SecurityDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}