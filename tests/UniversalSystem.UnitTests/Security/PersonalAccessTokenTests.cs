using BuildingBlocks.Domain.Authorization;
using BuildingBlocks.Domain.Exceptions;
using Security.Domain.Authorization;
using Security.Infrastructure.Authorization;
using Xunit;

namespace UniversalSystem.UnitTests.Security;

public class PersonalAccessTokenTests
{
    [Fact]
    public void Pat_Create_WithUnlimitedExpiration_IsActiveForever()
    {
        var rawToken = "pat_live_test_unlimited_token_1234567890";
        var hash = PatAuthenticationHandler.ComputeSha256(rawToken);

        var pat = PersonalAccessToken.Create(
            "YÖK Entegrasyonu",
            hash,
            "pat_live_test_u...7890",
            ["student.read", "course.read"],
            ScopeType.University,
            null,
            Guid.NewGuid(),
            null, // Sınırsız süre
            DateTime.UtcNow);

        Assert.Equal("YÖK Entegrasyonu", pat.Name);
        Assert.Null(pat.ExpiresAtUtc);
        Assert.True(pat.IsActive(DateTime.UtcNow.AddYears(10)));
        Assert.True(pat.HasPermission("student.read"));
        Assert.True(pat.HasPermission("course.read"));
        Assert.False(pat.HasPermission("student.create")); // Tanımlanmayan izin yok
    }

    [Fact]
    public void Pat_Create_WithFutureExpiration_ExpiresWhenTimePasses()
    {
        var rawToken = "pat_live_test_expiring_token_1234567890";
        var hash = PatAuthenticationHandler.ComputeSha256(rawToken);
        var created = DateTime.UtcNow;
        var expires = created.AddDays(7);

        var pat = PersonalAccessToken.Create(
            "Kütüphane Sistemi",
            hash,
            "pat_live_test_e...7890",
            ["student.read"],
            ScopeType.University,
            null,
            Guid.NewGuid(),
            expires,
            created);

        Assert.True(pat.IsActive(created.AddDays(1)));
        Assert.False(pat.IsActive(expires.AddMinutes(1))); // Süresi dolunca inaktif
    }

    [Fact]
    public void Pat_Revoke_MakesTokenInactive_AndCannotBeRevokedTwice()
    {
        var pat = PersonalAccessToken.Create(
            "Mobil Entegrasyon",
            "hash_abc",
            "pat_live_1234...abcd",
            ["course.read"],
            ScopeType.University,
            null,
            Guid.NewGuid(),
            null,
            DateTime.UtcNow);

        Assert.True(pat.IsActive(DateTime.UtcNow));

        pat.Revoke(Guid.NewGuid(), DateTime.UtcNow);
        Assert.False(pat.IsActive(DateTime.UtcNow));

        var ex = Assert.Throws<DomainException>(() => pat.Revoke(Guid.NewGuid(), DateTime.UtcNow));
        Assert.Equal("Pat.AlreadyRevoked", ex.Code);
    }

    [Fact]
    public void Pat_Create_RejectsPastExpiration_AndEmptyPermissions()
    {
        var now = DateTime.UtcNow;

        // Geçmiş tarihli son kullanma
        var exExpired = Assert.Throws<DomainException>(() => PersonalAccessToken.Create(
            "Geçersiz", "hash", "masked", ["student.read"], ScopeType.University, null, Guid.NewGuid(), now.AddHours(-1), now));
        Assert.Equal("Pat.ExpirationMustBeInFuture", exExpired.Code);

        // Boş izin listesi
        var exNoPerm = Assert.Throws<DomainException>(() => PersonalAccessToken.Create(
            "Geçersiz", "hash", "masked", [], ScopeType.University, null, Guid.NewGuid(), null, now));
        Assert.Equal("Pat.AtLeastOnePermissionRequired", exNoPerm.Code);
    }

    [Fact]
    public void Pat_Sha256_ProducesConsistentHash()
    {
        var token1 = "pat_live_my_secret_token_value_abc123";
        var hash1 = PatAuthenticationHandler.ComputeSha256(token1);
        var hash2 = PatAuthenticationHandler.ComputeSha256(token1);

        Assert.Equal(hash1, hash2);
        Assert.Equal(64, hash1.Length);
    }
}
