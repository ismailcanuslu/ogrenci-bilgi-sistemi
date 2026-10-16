using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;
using Security.Domain.Enums;
using Security.Domain.Errors;
using Security.Domain.ValueObjects;

namespace Security.Domain.Entities;

public sealed class User : AggregateRoot<Guid>
{
    private readonly List<PasskeyCredential> _passkeyCredentials = [];
    
    private readonly List<RecoveryCode> _recoveryCodes = [];

    public Email Email { get; private set; } = null!;

    public PasswordCredential? PasswordCredential { get; private set; }

    public IReadOnlyCollection<PasskeyCredential> PasskeyCredentials =>
        _passkeyCredentials.AsReadOnly();
    
    public IReadOnlyCollection<RecoveryCode> RecoveryCodes =>
        _recoveryCodes.AsReadOnly();
    
    public TotpCredential? TotpCredential { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? LastLoginAtUtc { get; private set; }
    
    public int FailedLoginAttemptCount { get; private set; }

    public DateTime? LockedUntilUtc { get; private set; }

    public bool IsActive { get; private set; }
    
    public bool IsLocked(DateTime nowUtc) =>
        LockedUntilUtc is not null &&
        LockedUntilUtc > nowUtc;

    private User()
    {
    }

    private User(
        Guid id,
        Email email,
        DateTime createdAtUtc)
        : base(id)
    {
        Email = email;
        CreatedAtUtc = createdAtUtc;
        IsActive = true;
    }

    public static User Create(
        Email email,
        DateTime createdAtUtc)
    {
        ArgumentNullException.ThrowIfNull(email);

        return new User(
            Guid.NewGuid(),
            email,
            createdAtUtc);
    }

    public void ChangeEmail(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);

        EnsureActive();

        Email = email;
    }

    public void SetPasswordCredential(
        string passwordHash,
        DateTime createdAtUtc)
    {
        EnsureActive();

        if (PasswordCredential is not null)
        {
            throw new DomainException(
                SecurityErrors.User.PasswordCredentialAlreadyExists);
        }

        PasswordCredential = PasswordCredential.Create(
            Id,
            passwordHash,
            createdAtUtc);
    }

    public void ChangePassword(
        string passwordHash,
        DateTime changedAtUtc)
    {
        EnsureActive();

        if (PasswordCredential is null)
        {
            throw new DomainException(
                SecurityErrors.User.PasswordCredentialNotFound);
        }

        PasswordCredential.ChangePassword(
            passwordHash,
            changedAtUtc);
    }

    public PasskeyCredential AddPasskeyCredential(
        byte[] credentialId,
        byte[] publicKey,
        byte[] userHandle,
        AuthenticatorType authenticatorType,
        string? name = null)
    {
        EnsureActive();

        if (_passkeyCredentials.Any(x =>
                x.CredentialId.AsSpan().SequenceEqual(credentialId)))
        {
            throw new DomainException(
                SecurityErrors.User.PasskeyCredentialAlreadyExists);
        }

        var credential = PasskeyCredential.Create(
            Id,
            credentialId,
            publicKey,
            userHandle,
            authenticatorType,
            name);

        _passkeyCredentials.Add(credential);

        return credential;
    }

    public void RevokePasskeyCredential(
        Guid credentialId,
        DateTimeOffset revokedAt)
    {
        EnsureActive();

        var credential = _passkeyCredentials
            .FirstOrDefault(x => x.Id == credentialId);

        if (credential is null)
        {
            throw new DomainException(
                SecurityErrors.User.PasskeyCredentialNotFound);
        }

        credential.Revoke(revokedAt);
    }
    
    public RecoveryCode AddRecoveryCode(
        byte[] codeHash,
        DateTime createdAtUtc)
    {
        EnsureActive();

        var recoveryCode = RecoveryCode.Create(
            Id,
            codeHash,
            createdAtUtc);

        _recoveryCodes.Add(recoveryCode);

        return recoveryCode;
    }
    
    public void SetTotpCredential(
        string protectedSecret,
        DateTime createdAtUtc)
    {
        EnsureActive();

        if (TotpCredential is not null)
        {
            throw new DomainException(
                SecurityErrors.User.TotpCredentialAlreadyExists);
        }

        TotpCredential = TotpCredential.Create(
            Id,
            protectedSecret,
            createdAtUtc);
    }
    
    public void EnableTotp(DateTime enabledAtUtc)
    {
        EnsureActive();

        if (TotpCredential is null)
        {
            throw new DomainException(
                SecurityErrors.User.TotpCredentialNotFound);
        }

        TotpCredential.Enable(enabledAtUtc);
    }
    
    public void DisableTotp(DateTime disabledAtUtc)
    {
        EnsureActive();

        if (TotpCredential is null)
        {
            throw new DomainException(
                SecurityErrors.User.TotpCredentialNotFound);
        }

        TotpCredential.Disable(disabledAtUtc);
    }
    
    public void EnablePasskeyMfa(Guid credentialId)
    {
        EnsureActive();

        var credential = _passkeyCredentials
            .FirstOrDefault(x => x.Id == credentialId);

        if (credential is null)
        {
            throw new DomainException(
                SecurityErrors.User.PasskeyCredentialNotFound);
        }

        credential.EnableMfa();
    }

    public void DisablePasskeyMfa(Guid credentialId)
    {
        EnsureActive();

        var credential = _passkeyCredentials
            .FirstOrDefault(x => x.Id == credentialId);

        if (credential is null)
        {
            throw new DomainException(
                SecurityErrors.User.PasskeyCredentialNotFound);
        }

        credential.DisableMfa();
    }

    public void RecordLogin(DateTime loginAtUtc)
    {
        EnsureActive();

        if (loginAtUtc < CreatedAtUtc)
        {
            throw new DomainException(
                SecurityErrors.User.LoginBeforeCreation);
        }

        LastLoginAtUtc = loginAtUtc;
    }
    
    public void RecordFailedLogin(
        DateTime failedAtUtc,
        int maxFailedAttempts,
        TimeSpan lockoutDuration)
    {
        EnsureActive();

        if (maxFailedAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxFailedAttempts));
        }

        if (lockoutDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lockoutDuration));
        }

        if (IsLocked(failedAtUtc))
        {
            throw new DomainException(
                SecurityErrors.User.Locked);
        }

        FailedLoginAttemptCount++;

        if (FailedLoginAttemptCount >= maxFailedAttempts)
        {
            LockedUntilUtc = failedAtUtc.Add(lockoutDuration);
        }
    }
    
    public void RecordSuccessfulLogin(DateTime loginAtUtc)
    {
        EnsureActive();

        if (IsLocked(loginAtUtc))
        {
            throw new DomainException(
                SecurityErrors.User.Locked);
        }

        if (loginAtUtc < CreatedAtUtc)
        {
            throw new DomainException(
                SecurityErrors.User.LoginBeforeCreation);
        }

        FailedLoginAttemptCount = 0;
        LockedUntilUtc = null;
        LastLoginAtUtc = loginAtUtc;
    }
    
    public void Unlock()
    {
        EnsureActive();

        FailedLoginAttemptCount = 0;
        LockedUntilUtc = null;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new DomainException(
                SecurityErrors.User.Inactive);
        }
    }
}