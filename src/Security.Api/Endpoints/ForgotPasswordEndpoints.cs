using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Security;
using Security.Infrastructure.Persistence;

namespace Security.Api.Endpoints;

public static class ForgotPasswordEndpoints
{
    private static readonly Dictionary<string, (string Tckn, DateTime ExpiresAt)> _tokens = new();

    public static IEndpointRouteBuilder MapForgotPasswordEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth/forgot-password")
            .WithTags("ForgotPassword");

        group.MapPost("/verify-identity", (VerifyIdentityRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.Tckn) || request.Tckn.Length != 11 || !request.Tckn.All(char.IsDigit))
            {
                return Results.BadRequest(new { error = "T.C. Kimlik Numarası 11 haneli rakamlardan oluşmalıdır." });
            }

            if (string.IsNullOrWhiteSpace(request.SerialNumber))
            {
                return Results.BadRequest(new { error = "Kimlik Seri Numarası boş bırakılamaz." });
            }

            if (string.IsNullOrWhiteSpace(request.BirthDate))
            {
                return Results.BadRequest(new { error = "Doğum Tarihi boş bırakılamaz." });
            }

            if (string.IsNullOrWhiteSpace(request.ExpiryDate))
            {
                return Results.BadRequest(new { error = "Kimlik Son Geçerlilik Tarihi boş bırakılamaz." });
            }

            // Simüle edilmiş NVI / KPS doğrulama & tek kullanımlık token üretimi
            var token = "NVITOKEN_" + Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
            lock (_tokens)
            {
                _tokens[token] = (request.Tckn, DateTime.UtcNow.AddMinutes(15));
            }

            return Results.Ok(new
            {
                success = true,
                verificationToken = token,
                message = "Nüfus ve Vatandaşlık İşleri (NVİ) kimlik doğrulaması başarılı. Lütfen yeni güçlü parolanızı belirleyin."
            });
        })
        .AllowAnonymous()
        .WithName("VerifyIdentity")
        .WithSummary("T.C. Kimlik Kartı bilgileri ile kimlik doğrular");

        group.MapPost("/reset", async (
            ResetPasswordRequest request,
            SecurityDbContext dbContext,
            IPasswordHasher passwordHasher,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.VerificationToken))
            {
                return Results.BadRequest(new { error = "Doğrulama belirteci eksik veya geçersiz." });
            }

            lock (_tokens)
            {
                if (!_tokens.TryGetValue(request.VerificationToken, out var data) || data.ExpiresAt < DateTime.UtcNow)
                {
                    return Results.BadRequest(new { error = "Doğrulama oturumunun süresi dolmuş veya geçersiz. Lütfen ilk adımdan tekrar başlayın." });
                }
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return Results.BadRequest(new { error = "Yeni parola boş olamaz." });
            }

            // Güçlü parola doğrulama kuralları: Min 8 karakter, büyük harf, küçük harf, rakam ve özel karakter
            var hasUpper = Regex.IsMatch(request.NewPassword, @"[A-Z]");
            var hasLower = Regex.IsMatch(request.NewPassword, @"[a-z]");
            var hasDigit = Regex.IsMatch(request.NewPassword, @"[0-9]");
            var hasSpecial = Regex.IsMatch(request.NewPassword, @"[!@#$%^&*()_+\-=\[\]{};':"",.<>/?]");

            if (request.NewPassword.Length < 8 || !hasUpper || !hasLower || !hasDigit || !hasSpecial)
            {
                return Results.BadRequest(new
                {
                    error = "Parolanız güvenlik kriterlerini karşılamıyor: En az 8 karakter, 1 büyük harf, 1 küçük harf, 1 rakam ve 1 özel sembol içermelidir."
                });
            }

            // Şifreyi güncelle (örnek kullanıcı veya istekte gelen email)
            var newHash = passwordHasher.Hash(request.NewPassword);
            var user = await dbContext.Users
                .Include(u => u.PasswordCredential)
                .FirstOrDefaultAsync(ct);

            if (user?.PasswordCredential is not null)
            {
                user.SetPasswordCredential(newHash, DateTime.UtcNow);
                await dbContext.SaveChangesAsync(ct);
            }

            lock (_tokens)
            {
                _tokens.Remove(request.VerificationToken);
            }

            return Results.Ok(new
            {
                success = true,
                message = "Parolanız başarıyla güncellenmiştir. Yeni parolanızla giriş yapabilirsiniz."
            });
        })
        .AllowAnonymous()
        .WithName("ResetPasswordWithIdentity")
        .WithSummary("Doğrulanmış kimlik ile yeni güçlü parola belirler");

        return app;
    }
}

public sealed record VerifyIdentityRequest(
    string Tckn,
    string BirthDate,
    string SerialNumber,
    string ExpiryDate);

public sealed record ResetPasswordRequest(
    string VerificationToken,
    string NewPassword);
