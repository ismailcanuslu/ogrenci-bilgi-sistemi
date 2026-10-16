namespace Security.Domain.Enums;

public enum TwoFactorType
{
    Totp = 1,
    Email = 2,
    Sms = 3,
    Passkey = 4
}