namespace Security.Domain.Errors;

public static class SecurityErrors
{
    public static class PasskeyCredential
    {
        public const string UserIdRequired =
            "security.passkey_credential.user_id_required";

        public const string CredentialIdRequired =
            "security.passkey_credential.credential_id_required";

        public const string PublicKeyRequired =
            "security.passkey_credential.public_key_required";

        public const string AuthenticationBeforeCreation =
            "security.passkey_credential.authentication_before_creation";

        public const string RevocationBeforeCreation =
            "security.passkey_credential.revocation_before_creation";

        public const string Revoked =
            "security.passkey_credential.revoked";
    }
    public static class PasskeyAuthenticationChallenge
    {
        public const string UserIdRequired =
            "security.passkey_authentication_challenge.user_id_required";

        public const string ExpiryBeforeCreation =
            "security.passkey_authentication_challenge.expiry_before_creation";

        public const string Expired =
            "security.passkey_authentication_challenge.expired";

        public const string AlreadyUsed =
            "security.passkey_authentication_challenge.already_used";

        public const string UsedBeforeCreation =
            "security.passkey_authentication_challenge.used_before_creation";
        
        public const string OptionsRequired =
            "security.passkey_authentication_challenge.options_required";
    }
    public static class PasskeyRegistrationChallenge
    {
        public const string UserIdRequired =
            "security.passkey_registration_challenge.user_id_required";

        public const string OptionsRequired =
            "security.passkey_registration_challenge.options_required";

        public const string ExpiryBeforeCreation =
            "security.passkey_registration_challenge.expiry_before_creation";

        public const string Expired =
            "security.passkey_registration_challenge.expired";

        public const string AlreadyUsed =
            "security.passkey_registration_challenge.already_used";

        public const string UsedBeforeCreation =
            "security.passkey_registration_challenge.used_before_creation";
    }
    public static class PasswordCredential
    {
        public const string UserIdRequired =
            "security.password_credential.user_id_required";

        public const string PasswordHashRequired =
            "security.password_credential.password_hash_required";

        public const string UsedBeforeCreation =
            "security.password_credential.used_before_creation";
    }
    public static class Email
    {
        public const string Required =
            "security.email.required";

        public const string Invalid =
            "security.email.invalid";
    }
    public static class User
    {
        public const string PasswordCredentialAlreadyExists =
            "security.user.password_credential_already_exists";

        public const string PasswordCredentialNotFound =
            "security.user.password_credential_not_found";

        public const string PasskeyCredentialAlreadyExists =
            "security.user.passkey_credential_already_exists";

        public const string PasskeyCredentialNotFound =
            "security.user.passkey_credential_not_found";
        public const string TotpCredentialAlreadyExists =
            "security.user.totp_credential_already_exists";

        public const string TotpCredentialNotFound =
            "security.user.totp_credential_not_found";

        public const string LoginBeforeCreation =
            "security.user.login_before_creation";
        
        public const string Locked =
            "security.user.locked";

        public const string Inactive =
            "security.user.inactive";
    }
    public static class EmailVerificationChallenge
{
    public const string UserIdRequired =
        "security.email_verification_challenge.user_id_required";

    public const string CodeHashRequired =
        "security.email_verification_challenge.code_hash_required";

    public const string ExpiryBeforeCreation =
        "security.email_verification_challenge.expiry_before_creation";

    public const string Expired =
        "security.email_verification_challenge.expired";

    public const string AlreadyUsed =
        "security.email_verification_challenge.already_used";

    public const string InvalidCode =
        "security.email_verification_challenge.invalid_code";

    public const string MaxAttemptsExceeded =
        "security.email_verification_challenge.max_attempts_exceeded";
}
    public static class EmailLoginChallenge
{
    public const string UserIdRequired =
        "security.email_login_challenge.user_id_required";

    public const string CodeHashRequired =
        "security.email_login_challenge.code_hash_required";

    public const string ExpiryBeforeCreation =
        "security.email_login_challenge.expiry_before_creation";

    public const string Expired =
        "security.email_login_challenge.expired";

    public const string AlreadyUsed =
        "security.email_login_challenge.already_used";

    public const string InvalidCode =
        "security.email_login_challenge.invalid_code";

    public const string MaxAttemptsExceeded =
        "security.email_login_challenge.max_attempts_exceeded";
}
    public static class PasswordResetChallenge
{
    public const string UserIdRequired =
        "security.password_reset_challenge.user_id_required";

    public const string CodeHashRequired =
        "security.password_reset_challenge.code_hash_required";

    public const string ExpiryBeforeCreation =
        "security.password_reset_challenge.expiry_before_creation";

    public const string Expired =
        "security.password_reset_challenge.expired";

    public const string AlreadyUsed =
        "security.password_reset_challenge.already_used";

    public const string InvalidCode =
        "security.password_reset_challenge.invalid_code";

    public const string MaxAttemptsExceeded =
        "security.password_reset_challenge.max_attempts_exceeded";
}
    public static class TotpCredential
    {
        public const string UserIdRequired =
            "security.totp_credential.user_id_required";

        public const string ProtectedSecretRequired =
            "security.totp_credential.protected_secret_required";

        public const string EnabledBeforeCreation =
            "security.totp_credential.enabled_before_creation";

        public const string AlreadyEnabled =
            "security.totp_credential.already_enabled";

        public const string Disabled =
            "security.totp_credential.disabled";

        public const string NotEnabled =
            "security.totp_credential.not_enabled";

        public const string UsedBeforeEnabled =
            "security.totp_credential.used_before_enabled";

        public const string DisabledBeforeEnabled =
            "security.totp_credential.disabled_before_enabled";
        
        public const string VerifiedBeforeEnabled =
            "security.totp_credential.verified_before_enabled";
    }
    public static class EmailMfaChallenge
    {
        public const string UserIdRequired =
            "security.email_mfa_challenge.user_id_required";

        public const string CodeHashRequired =
            "security.email_mfa_challenge.code_hash_required";

        public const string ExpiryBeforeCreation =
            "security.email_mfa_challenge.expiry_before_creation";

        public const string Expired =
            "security.email_mfa_challenge.expired";

        public const string AlreadyUsed =
            "security.email_mfa_challenge.already_used";

        public const string InvalidCode =
            "security.email_mfa_challenge.invalid_code";

        public const string MaxAttemptsExceeded =
            "security.email_mfa_challenge.max_attempts_exceeded";
    }
    public static class PhoneNumber
    {
        public const string Required =
            "security.phone_number.required";

        public const string Invalid =
            "security.phone_number.invalid";
    }
    public static class SmsMfaChallenge
    {
        public const string UserIdRequired =
            "security.sms_mfa_challenge.user_id_required";

        public const string CodeHashRequired =
            "security.sms_mfa_challenge.code_hash_required";

        public const string ExpiryBeforeCreation =
            "security.sms_mfa_challenge.expiry_before_creation";

        public const string Expired =
            "security.sms_mfa_challenge.expired";

        public const string AlreadyUsed =
            "security.sms_mfa_challenge.already_used";

        public const string InvalidCode =
            "security.sms_mfa_challenge.invalid_code";

        public const string MaxAttemptsExceeded =
            "security.sms_mfa_challenge.max_attempts_exceeded";
    }
    public static class RecoveryCode
    {
        public const string UserIdRequired =
            "security.recovery_code.user_id_required";

        public const string CodeHashRequired =
            "security.recovery_code.code_hash_required";

        public const string AlreadyUsed =
            "security.recovery_code.already_used";

        public const string UsedBeforeCreation =
            "security.recovery_code.used_before_creation";

        public const string InvalidCode =
            "security.recovery_code.invalid_code";
    }
    public static class Session
    {
        public const string UserIdRequired =
            "security.session.user_id_required";

        public const string ExpiryBeforeCreation =
            "security.session.expiry_before_creation";

        public const string Expired =
            "security.session.expired";

        public const string Revoked =
            "security.session.revoked";

        public const string RevokedBeforeCreation =
            "security.session.revoked_before_creation";

        public const string RefreshTokenNotFound =
            "security.session.refresh_token_not_found";
    }
    public static class RefreshToken
    {
        public const string SessionIdRequired =
            "security.refresh_token.session_id_required";

        public const string TokenHashRequired =
            "security.refresh_token.token_hash_required";

        public const string ExpiryBeforeCreation =
            "security.refresh_token.expiry_before_creation";

        public const string Expired =
            "security.refresh_token.expired";

        public const string Revoked =
            "security.refresh_token.revoked";

        public const string AlreadyUsed =
            "security.refresh_token.already_used";

        public const string InvalidToken =
            "security.refresh_token.invalid_token";

        public const string UsedBeforeCreation =
            "security.refresh_token.used_before_creation";

        public const string ReplacementTokenRequired =
            "security.refresh_token.replacement_token_required";

        public const string RevokedBeforeCreation =
            "security.refresh_token.revoked_before_creation";
    }
}