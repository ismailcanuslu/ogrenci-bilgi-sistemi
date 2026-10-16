using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Security.Infrastructure.Persistence;

namespace Security.Infrastructure.Authorization;

public static class PatAuthenticationDefaults
{
    public const string AuthenticationScheme = "PatToken";
    public const string HeaderName = "X-PatKey";
    public const string AltHeaderName = "X-Pat-Token";
    public const string TokenPrefix = "pat_live_";
}

public sealed class PatAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    SecurityDbContext dbContext)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? rawToken = null;

        if (Request.Headers.TryGetValue(PatAuthenticationDefaults.HeaderName, out var headerVal) &&
            !string.IsNullOrWhiteSpace(headerVal))
        {
            rawToken = headerVal.ToString().Trim();
        }
        else if (Request.Headers.TryGetValue(PatAuthenticationDefaults.AltHeaderName, out var altHeaderVal) &&
                 !string.IsNullOrWhiteSpace(altHeaderVal))
        {
            rawToken = altHeaderVal.ToString().Trim();
        }
        else if (Request.Headers.TryGetValue("Authorization", out var authHeaderVal) &&
                 !string.IsNullOrWhiteSpace(authHeaderVal))
        {
            var header = authHeaderVal.ToString().Trim();
            if (header.StartsWith("Bearer " + PatAuthenticationDefaults.TokenPrefix, StringComparison.OrdinalIgnoreCase))
            {
                rawToken = header["Bearer ".Length..].Trim();
            }
        }

        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return AuthenticateResult.NoResult();
        }

        if (!rawToken.StartsWith(PatAuthenticationDefaults.TokenPrefix, StringComparison.Ordinal))
        {
            return AuthenticateResult.Fail("Invalid PAT key format.");
        }

        var hash = ComputeSha256(rawToken);
        var nowUtc = DateTime.UtcNow;

        var pat = await dbContext.PersonalAccessTokens
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.TokenHash == hash, Context.RequestAborted);

        if (pat is null || !pat.IsActive(nowUtc))
        {
            return AuthenticateResult.Fail("PAT key is invalid, expired, or revoked.");
        }

        pat.RecordUsage(nowUtc);
        try
        {
            await dbContext.SaveChangesAsync(Context.RequestAborted);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to record PAT last used timestamp.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, pat.Id.ToString()),
            new("sub", pat.Id.ToString()),
            new("pat_id", pat.Id.ToString()),
            new("is_pat", "true"),
            new("created_by", pat.CreatedByUserId.ToString()),
            new("scope_type", pat.ScopeType.ToString()),
            new(ClaimTypes.Role, "PatClient")
        };

        if (pat.ScopeId.HasValue)
        {
            claims.Add(new("scope_id", pat.ScopeId.Value.ToString()));
        }

        foreach (var p in pat.Permissions)
        {
            claims.Add(new("pat_permission", p.Permission));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    public static string ComputeSha256(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
