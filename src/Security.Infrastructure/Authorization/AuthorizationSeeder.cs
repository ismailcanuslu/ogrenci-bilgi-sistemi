using BuildingBlocks.Domain.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Security.Application.Abstractions.Security;
using Security.Domain.Authorization;
using Security.Infrastructure.Persistence;

namespace Security.Infrastructure.Authorization;

/// <summary>
/// Varsayılan sistem rollerini ve (opsiyonel) ilk yönetici atamasını idempotent şekilde oluşturur.
/// Mock öğrenci/test verisi üretmez; yalnızca referans (rol) verisi içerir.
/// </summary>
internal sealed class AuthorizationSeeder(
    IServiceScopeFactory scopeFactory,
    IOptions<AuthorizationSeedOptions> options,
    IPasswordHasher passwordHasher,
    ILogger<AuthorizationSeeder> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<SecurityDbContext>();

            await SeedRolesAsync(db, cancellationToken);
            await SeedDefaultUsersAsync(db, cancellationToken);
            await SeedBootstrapAdministratorAsync(db, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // Veritabanı henüz migrate edilmemiş olabilir; uygulamanın açılışını engelleme.
            logger.LogWarning(exception, "Authorization seed çalıştırılamadı.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task SeedRolesAsync(SecurityDbContext db, CancellationToken ct)
    {
        var existing = await db.Roles.Select(r => r.Name).ToListAsync(ct);

        foreach (var (name, permissions) in DefaultRoles.Definitions)
        {
            if (existing.Contains(name))
            {
                continue;
            }

            db.Roles.Add(Role.Create(name, null, permissions, DateTime.UtcNow, isSystem: true));
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task SeedDefaultUsersAsync(SecurityDbContext db, CancellationToken ct)
    {
        var defaultUsers = new (string Email, string[] Roles)[]
        {
            ("ogrenci@obs.edu.tr", [DefaultRoles.Student]),
            ("akademik@obs.edu.tr", [DefaultRoles.Instructor]),
            ("idari@obs.edu.tr", [DefaultRoles.StudentAffairs]),
            ("admin@obs.edu.tr", [DefaultRoles.SystemAdministrator, DefaultRoles.StudentAffairs, DefaultRoles.Instructor, DefaultRoles.Student]),
            ("arastirma.gorevlisi@obs.edu.tr", [DefaultRoles.Instructor, DefaultRoles.Student])
        };

        const string defaultPassword = "Obs123456!*";
        var passwordHash = passwordHasher.Hash(defaultPassword);
        var nowUtc = DateTime.UtcNow;

        foreach (var item in defaultUsers)
        {
            var userEmail = global::Security.Domain.ValueObjects.Email.Create(item.Email);
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email.Value == item.Email, ct);
            if (user is null)
            {
                user = global::Security.Domain.Entities.User.Create(userEmail, nowUtc);
                user.SetPasswordCredential(passwordHash, nowUtc);
                db.Users.Add(user);
                await db.SaveChangesAsync(ct);
            }

            foreach (var roleName in item.Roles)
            {
                var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == roleName, ct);
                if (role is not null)
                {
                    var hasAssignment = await db.UserRoleAssignments.AnyAsync(
                        a => a.UserId == user.Id && a.RoleId == role.Id && a.RevokedAtUtc == null,
                        ct);

                    if (!hasAssignment)
                    {
                        db.UserRoleAssignments.Add(
                            UserRoleAssignment.Create(user.Id, role.Id, ScopeType.University, null, nowUtc, null));
                        await db.SaveChangesAsync(ct);
                    }
                }
            }
        }
    }

    private async Task SeedBootstrapAdministratorAsync(SecurityDbContext db, CancellationToken ct)
    {
        var email = options.Value.BootstrapSystemAdministratorEmail?.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(email))
        {
            return;
        }

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email.Value == email, ct);
        var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == DefaultRoles.SystemAdministrator, ct);

        if (user is null || role is null)
        {
            logger.LogWarning("Bootstrap yöneticisi atanamadı: kullanıcı veya rol bulunamadı.");
            return;
        }

        var alreadyAssigned = await db.UserRoleAssignments.AnyAsync(
            a => a.UserId == user.Id
                 && a.RoleId == role.Id
                 && a.ScopeType == ScopeType.University
                 && a.RevokedAtUtc == null,
            ct);

        if (alreadyAssigned)
        {
            return;
        }

        db.UserRoleAssignments.Add(
            UserRoleAssignment.Create(user.Id, role.Id, ScopeType.University, null, DateTime.UtcNow, null));

        await db.SaveChangesAsync(ct);
    }
}
