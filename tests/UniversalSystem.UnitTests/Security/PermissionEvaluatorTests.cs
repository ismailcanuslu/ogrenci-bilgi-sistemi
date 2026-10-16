using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Domain.Authorization;
using BuildingBlocks.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Security.Domain.Authorization;
using Security.Domain.Entities;
using Security.Domain.ValueObjects;
using Security.Infrastructure.Authorization;
using Security.Infrastructure.Persistence;
using Xunit;

namespace UniversalSystem.UnitTests.Security;

public class PermissionEvaluatorTests : IDisposable
{
    private const string Permission = "department.update";

    private readonly SecurityDbContext _db;
    private readonly PermissionEvaluator _evaluator;

    public PermissionEvaluatorTests()
    {
        _db = new SecurityDbContext(
            new DbContextOptionsBuilder<SecurityDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        _evaluator = new PermissionEvaluator(_db);
    }

    public void Dispose() => _db.Dispose();

    private async Task<(User User, Role Role)> SeedAsync(string[]? permissions = null)
    {
        var user = User.Create(Email.Create($"{Guid.NewGuid():N}@obs.edu.tr"), DateTime.UtcNow);
        var role = Role.Create($"Role-{Guid.NewGuid():N}", null, permissions ?? [Permission], DateTime.UtcNow);
        _db.Users.Add(user);
        _db.Roles.Add(role);
        await _db.SaveChangesAsync();
        return (user, role);
    }

    private async Task<UserRoleAssignment> AssignAsync(User user, Role role, ScopeType type, Guid? scopeId)
    {
        var assignment = UserRoleAssignment.Create(user.Id, role.Id, type, scopeId, DateTime.UtcNow, null);
        _db.UserRoleAssignments.Add(assignment);
        await _db.SaveChangesAsync();
        return assignment;
    }

    [Fact]
    public async Task DepartmentScopedGrant_AllowsOnlyThatDepartment()
    {
        var (user, role) = await SeedAsync();
        var own = Guid.NewGuid();
        await AssignAsync(user, role, ScopeType.Department, own);

        Assert.True(await _evaluator.HasPermissionAsync(user.Id, Permission, [ResourceScope.Department(own)]));
        Assert.False(await _evaluator.HasPermissionAsync(user.Id, Permission, [ResourceScope.Department(Guid.NewGuid())]));
    }

    [Fact]
    public async Task FacultyScopedGrant_AppliesWhenFacultyIsAmongCandidateScopes()
    {
        var (user, role) = await SeedAsync();
        var faculty = Guid.NewGuid();
        await AssignAsync(user, role, ScopeType.Faculty, faculty);

        var allowed = await _evaluator.HasPermissionAsync(
            user.Id, Permission, [ResourceScope.Department(Guid.NewGuid()), ResourceScope.Faculty(faculty)]);
        var denied = await _evaluator.HasPermissionAsync(
            user.Id, Permission, [ResourceScope.Department(Guid.NewGuid()), ResourceScope.Faculty(Guid.NewGuid())]);

        Assert.True(allowed);
        Assert.False(denied);
    }

    [Fact]
    public async Task UniversityScopedGrant_AllowsAnyScope()
    {
        var (user, role) = await SeedAsync();
        await AssignAsync(user, role, ScopeType.University, null);

        Assert.True(await _evaluator.HasPermissionAsync(user.Id, Permission, [ResourceScope.Department(Guid.NewGuid())]));
        Assert.True(await _evaluator.HasPermissionAsync(user.Id, Permission, [ResourceScope.University]));
    }

    [Fact]
    public async Task RevokedAssignment_NoLongerGrantsAccess()
    {
        var (user, role) = await SeedAsync();
        var deptId = Guid.NewGuid();
        var assignment = await AssignAsync(user, role, ScopeType.Department, deptId);

        Assert.True(await _evaluator.HasPermissionAsync(user.Id, Permission, [ResourceScope.Department(deptId)]));

        assignment.Revoke(DateTime.UtcNow, null, "test");
        await _db.SaveChangesAsync();

        Assert.False(await _evaluator.HasPermissionAsync(user.Id, Permission, [ResourceScope.Department(deptId)]));
        Assert.NotNull(await _db.UserRoleAssignments.FindAsync(assignment.Id)); // geçmiş korunur
    }

    [Fact]
    public async Task ReadOnlyRole_DoesNotGrantWritePermission()
    {
        var (user, role) = await SeedAsync(["department.read"]);
        var deptId = Guid.NewGuid();
        await AssignAsync(user, role, ScopeType.Department, deptId);

        Assert.False(await _evaluator.HasPermissionAsync(user.Id, Permission, [ResourceScope.Department(deptId)]));
        Assert.True(await _evaluator.HasPermissionAsync(user.Id, "department.read", [ResourceScope.Department(deptId)]));
    }

    [Fact]
    public async Task InactiveUser_IsAlwaysDenied()
    {
        var (user, role) = await SeedAsync();
        await AssignAsync(user, role, ScopeType.University, null);

        user.Deactivate();
        await _db.SaveChangesAsync();

        Assert.False(await _evaluator.HasPermissionAsync(user.Id, Permission, [ResourceScope.University]));
        Assert.Empty(await _evaluator.GetGrantedScopesAsync(user.Id, Permission));
    }

    [Fact]
    public async Task UnknownUser_IsDenied()
    {
        Assert.False(await _evaluator.HasPermissionAsync(Guid.NewGuid(), Permission, [ResourceScope.University]));
    }

    [Fact]
    public async Task GetGrantedScopes_ReturnsAllActiveScopesForPermission()
    {
        var (user, role) = await SeedAsync();
        var f1 = Guid.NewGuid();
        var d1 = Guid.NewGuid();
        await AssignAsync(user, role, ScopeType.Faculty, f1);
        await AssignAsync(user, role, ScopeType.Department, d1);

        var scopes = await _evaluator.GetGrantedScopesAsync(user.Id, Permission);

        Assert.Contains(ResourceScope.Faculty(f1), scopes);
        Assert.Contains(ResourceScope.Department(d1), scopes);
        Assert.Equal(2, scopes.Count);
    }

    [Theory]
    [InlineData(ScopeType.University, true)]
    [InlineData(ScopeType.Faculty, false)]
    [InlineData(ScopeType.Department, false)]
    [InlineData(ScopeType.Course, false)]
    [InlineData(ScopeType.Self, true)]
    public void Assignment_ScopeIdRules(ScopeType type, bool scopeIdMustBeNull)
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        if (scopeIdMustBeNull)
        {
            Assert.NotNull(UserRoleAssignment.Create(userId, roleId, type, null, DateTime.UtcNow, null));
            Assert.Throws<DomainException>(
                () => UserRoleAssignment.Create(userId, roleId, type, Guid.NewGuid(), DateTime.UtcNow, null));
        }
        else
        {
            Assert.NotNull(UserRoleAssignment.Create(userId, roleId, type, Guid.NewGuid(), DateTime.UtcNow, null));
            Assert.Throws<DomainException>(
                () => UserRoleAssignment.Create(userId, roleId, type, null, DateTime.UtcNow, null));
        }
    }

    [Fact]
    public void Assignment_CannotBeRevokedTwice()
    {
        var assignment = UserRoleAssignment.Create(
            Guid.NewGuid(), Guid.NewGuid(), ScopeType.University, null, DateTime.UtcNow, null);
        assignment.Revoke(DateTime.UtcNow, null, null);

        Assert.Throws<DomainException>(() => assignment.Revoke(DateTime.UtcNow, null, null));
    }

    [Fact]
    public void Role_NormalizesAndDeduplicatesPermissions()
    {
        var role = Role.Create("R", null, [" Department.Read ", "department.read", "department.update"], DateTime.UtcNow);

        Assert.Equal(2, role.Permissions.Count);
        Assert.True(role.HasPermission("department.read"));
    }
}
