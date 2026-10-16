using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Departments;
using Academic.Domain.Faculties;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Domain.Authorization;
using NSubstitute;

namespace UniversalSystem.UnitTests.Academic;

internal sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
{
    public Guid? UserId { get; } = userId;
}

internal sealed class FakePermissionEvaluator : IPermissionEvaluator
{
    private readonly List<(string Permission, ResourceScope Scope)> _grants = [];

    public FakePermissionEvaluator Grant(string permission, ResourceScope scope)
    {
        _grants.Add((permission, scope));
        return this;
    }

    public Task<bool> HasPermissionAsync(
        Guid userId, string permission, IReadOnlyCollection<ResourceScope> candidateScopes, CancellationToken cancellationToken = default)
        => Task.FromResult(_grants.Any(g =>
            g.Permission == permission &&
            (g.Scope.Type == ScopeType.University ||
             candidateScopes.Any(c => c.Type == g.Scope.Type && c.Id == g.Scope.Id))));

    public Task<IReadOnlyCollection<ResourceScope>> GetGrantedScopesAsync(
        Guid userId, string permission, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyCollection<ResourceScope>>(
            _grants.Where(g => g.Permission == permission).Select(g => g.Scope).ToArray());
}

internal sealed class InMemoryFacultyRepository : IFacultyRepository
{
    public List<Faculty> Items { get; } = [];

    public Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

    public Task<bool> CodeExistsAsync(string code, Guid? excludeId, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.Any(x => x.Code == code && x.Id != excludeId));

    public Task AddAsync(Faculty faculty, CancellationToken cancellationToken = default)
    {
        Items.Add(faculty);
        return Task.CompletedTask;
    }

    public Task<PagedResult<Faculty>> ListAsync(FacultyListCriteria criteria, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}

internal sealed class InMemoryDepartmentRepository : IDepartmentRepository
{
    public List<Department> Items { get; } = [];

    public DepartmentListCriteria? LastCriteria { get; private set; }

    public Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

    public Task<bool> CodeExistsInFacultyAsync(
        Guid facultyId, string code, Guid? excludeId, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.Any(x => x.FacultyId == facultyId && x.Code == code && x.Id != excludeId));

    public Task<bool> HasActiveDepartmentsAsync(Guid facultyId, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.Any(x => x.FacultyId == facultyId && x.IsActive));

    public Task AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        Items.Add(department);
        return Task.CompletedTask;
    }

    public Task<PagedResult<Department>> ListAsync(
        DepartmentListCriteria criteria, CancellationToken cancellationToken = default)
    {
        LastCriteria = criteria;
        return Task.FromResult(new PagedResult<Department>([], criteria.Page, criteria.PageSize, 0));
    }
}

/// <summary>Handler testleri için ortak kurulum: gerçek PermissionGuard/AcademicAccess + sahte altyapı.</summary>
internal sealed class AcademicFixture
{
    public Guid UserId { get; } = Guid.NewGuid();

    public FakePermissionEvaluator Evaluator { get; } = new();

    public InMemoryFacultyRepository Faculties { get; } = new();

    public InMemoryDepartmentRepository Departments { get; } = new();

    public IAcademicUnitOfWork UnitOfWork { get; } = Substitute.For<IAcademicUnitOfWork>();

    public IAuditService Audit { get; } = Substitute.For<IAuditService>();

    public AcademicAccess CreateAccess(bool authenticated = true)
    {
        var currentUser = new FakeCurrentUser(authenticated ? UserId : null);
        return new AcademicAccess(new PermissionGuard(currentUser, Evaluator), currentUser, Evaluator);
    }

    public Faculty AddFaculty(string code = "ENG")
    {
        var faculty = Faculty.Create(code, $"{code} Fakültesi", DateTime.UtcNow);
        Faculties.Items.Add(faculty);
        return faculty;
    }

    public Department AddDepartment(Faculty faculty, string code = "CSE")
    {
        var department = Department.Create(faculty.Id, code, $"{code} Bölümü", DateTime.UtcNow);
        Departments.Items.Add(department);
        return department;
    }
}
