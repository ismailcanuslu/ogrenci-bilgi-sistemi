using Academic.Application.Features.Departments;
using Academic.Application.Features.Faculties;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using NSubstitute;
using Xunit;

namespace UniversalSystem.UnitTests.Academic;

public class AcademicAuthorizationTests
{
    private readonly AcademicFixture _f = new();

    private UpdateDepartment.Handler UpdateDeptHandler(bool authenticated = true)
        => new(_f.CreateAccess(authenticated), _f.Departments, _f.UnitOfWork, _f.Audit);

    private CreateDepartment.Handler CreateDeptHandler()
        => new(_f.CreateAccess(), _f.Faculties, _f.Departments, _f.UnitOfWork, _f.Audit);

    private CreateFaculty.Handler CreateFacultyHandler()
        => new(_f.CreateAccess(), _f.Faculties, _f.UnitOfWork, _f.Audit);

    private async Task AssertNoSideEffects()
    {
        await _f.UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _f.Audit.DidNotReceiveWithAnyArgs().RecordAsync(default!, default!, default!);
    }

    [Fact]
    public async Task DepartmentManager_CanUpdate_OwnDepartment()
    {
        var faculty = _f.AddFaculty();
        var own = _f.AddDepartment(faculty, "CSE");
        _f.Evaluator.Grant(Permissions.Department.Update, ResourceScope.Department(own.Id));

        var result = await UpdateDeptHandler().Handle(new UpdateDepartment.Command(own.Id, "CSE", "Yeni Ad"), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("Yeni Ad", own.Name);
        await _f.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _f.Audit.Received(1).RecordAsync(
            "Department.Updated", "Academic", "Department", own.Id.ToString(),
            Arg.Any<object?>(), Arg.Any<object?>(), Arg.Any<bool>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DepartmentManager_CannotUpdate_AnotherDepartment_EvenInSameFaculty()
    {
        var faculty = _f.AddFaculty();
        var own = _f.AddDepartment(faculty, "CSE");
        var other = _f.AddDepartment(faculty, "EEE");
        _f.Evaluator.Grant(Permissions.Department.Update, ResourceScope.Department(own.Id));

        var result = await UpdateDeptHandler().Handle(new UpdateDepartment.Command(other.Id, "EEE", "Hack"), default);

        Assert.Equal(403, result.StatusCode);
        Assert.Equal("EEE Bölümü", other.Name);
        await AssertNoSideEffects();
    }

    [Fact]
    public async Task DepartmentManager_CannotCreate_DepartmentInOwnFaculty()
    {
        var faculty = _f.AddFaculty();
        var own = _f.AddDepartment(faculty);
        _f.Evaluator.Grant(Permissions.Department.Create, ResourceScope.Department(own.Id));

        var result = await CreateDeptHandler().Handle(new CreateDepartment.Command(faculty.Id, "NEW", "Yeni"), default);

        Assert.Equal(403, result.StatusCode);
        await AssertNoSideEffects();
    }

    [Fact]
    public async Task FacultyManager_CanManage_DepartmentsOfOwnFaculty()
    {
        var faculty = _f.AddFaculty();
        var dept = _f.AddDepartment(faculty);
        _f.Evaluator
            .Grant(Permissions.Department.Update, ResourceScope.Faculty(faculty.Id))
            .Grant(Permissions.Department.Create, ResourceScope.Faculty(faculty.Id));

        var update = await UpdateDeptHandler().Handle(new UpdateDepartment.Command(dept.Id, "CSE", "Guncel"), default);
        var create = await CreateDeptHandler().Handle(new CreateDepartment.Command(faculty.Id, "NEW", "Yeni"), default);

        Assert.True(update.IsSuccess);
        Assert.Equal(201, create.StatusCode);
    }

    [Fact]
    public async Task FacultyManager_CannotManage_AnotherFacultysRecords()
    {
        var own = _f.AddFaculty("ENG");
        var otherFaculty = _f.AddFaculty("MED");
        var otherDept = _f.AddDepartment(otherFaculty, "ANA");
        _f.Evaluator
            .Grant(Permissions.Department.Update, ResourceScope.Faculty(own.Id))
            .Grant(Permissions.Department.Create, ResourceScope.Faculty(own.Id))
            .Grant(Permissions.Faculty.Update, ResourceScope.Faculty(own.Id));

        var update = await UpdateDeptHandler().Handle(new UpdateDepartment.Command(otherDept.Id, "ANA", "Hack"), default);
        var create = await CreateDeptHandler().Handle(new CreateDepartment.Command(otherFaculty.Id, "NEW", "Hack"), default);
        var faculty = await new UpdateFaculty.Handler(_f.CreateAccess(), _f.Faculties, _f.UnitOfWork, _f.Audit)
            .Handle(new UpdateFaculty.Command(otherFaculty.Id, "MED", "Hack"), default);

        Assert.Equal(403, update.StatusCode);
        Assert.Equal(403, create.StatusCode);
        Assert.Equal(403, faculty.StatusCode);
        await AssertNoSideEffects();
    }

    [Fact]
    public async Task ReadOnlyUser_CannotModify()
    {
        var faculty = _f.AddFaculty();
        var dept = _f.AddDepartment(faculty);
        _f.Evaluator
            .Grant(Permissions.Department.Read, ResourceScope.University)
            .Grant(Permissions.Faculty.Read, ResourceScope.University);

        var result = await UpdateDeptHandler().Handle(new UpdateDepartment.Command(dept.Id, "CSE", "Hack"), default);

        Assert.Equal(403, result.StatusCode);
        await AssertNoSideEffects();
    }

    [Fact]
    public async Task StudentAffairs_CanCreateFaculty_ButFacultyManagerCannot()
    {
        var faculty = _f.AddFaculty("ENG");
        _f.Evaluator.Grant(Permissions.Faculty.Update, ResourceScope.Faculty(faculty.Id));

        var denied = await CreateFacultyHandler().Handle(new CreateFaculty.Command("NEW", "Yeni"), default);
        Assert.Equal(403, denied.StatusCode);

        _f.Evaluator.Grant(Permissions.Faculty.Create, ResourceScope.University);
        var allowed = await CreateFacultyHandler().Handle(new CreateFaculty.Command("NEW", "Yeni"), default);
        Assert.Equal(201, allowed.StatusCode);
    }

    [Fact]
    public async Task UnauthenticatedCaller_GetsUnauthorized_AndNothingChanges()
    {
        var faculty = _f.AddFaculty();
        var dept = _f.AddDepartment(faculty);
        _f.Evaluator.Grant(Permissions.Department.Update, ResourceScope.University);

        var result = await UpdateDeptHandler(authenticated: false)
            .Handle(new UpdateDepartment.Command(dept.Id, "CSE", "Hack"), default);

        Assert.Equal(401, result.StatusCode);
        await AssertNoSideEffects();
    }

    [Fact]
    public async Task MissingDepartment_DoesNotLeakExistence_ToScopedUsers()
    {
        var faculty = _f.AddFaculty();
        var own = _f.AddDepartment(faculty);
        _f.Evaluator.Grant(Permissions.Department.Update, ResourceScope.Department(own.Id));

        var result = await UpdateDeptHandler().Handle(
            new UpdateDepartment.Command(Guid.NewGuid(), "X", "X"), default);

        Assert.Equal(403, result.StatusCode);
    }

    [Fact]
    public async Task MissingDepartment_UniversityScopedUser_GetsNotFound()
    {
        _f.Evaluator.Grant(Permissions.Department.Update, ResourceScope.University);

        var result = await UpdateDeptHandler().Handle(
            new UpdateDepartment.Command(Guid.NewGuid(), "X", "X"), default);

        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Department.NotFound", result.Errors.Single().Code);
    }

    [Fact]
    public async Task ListDepartments_FacultyManager_IsRestrictedToOwnFaculty()
    {
        var faculty = _f.AddFaculty();
        _f.Evaluator.Grant(Permissions.Department.Read, ResourceScope.Faculty(faculty.Id));

        var handler = new ListDepartments.Handler(_f.CreateAccess(), _f.Departments);
        var result = await handler.Handle(new ListDepartments.Query(), default);

        Assert.True(result.IsSuccess);
        var criteria = _f.Departments.LastCriteria!;
        Assert.False(criteria.Unrestricted);
        Assert.Equal([faculty.Id], criteria.AllowedFacultyIds);
        Assert.Empty(criteria.AllowedDepartmentIds);
    }

    [Fact]
    public async Task ListDepartments_UserWithoutAnyGrant_IsForbidden()
    {
        var handler = new ListDepartments.Handler(_f.CreateAccess(), _f.Departments);
        var result = await handler.Handle(new ListDepartments.Query(), default);

        Assert.Equal(403, result.StatusCode);
        Assert.Null(_f.Departments.LastCriteria);
    }

    [Fact]
    public async Task DepartmentManager_CannotRead_FacultyOfOwnDepartment()
    {
        var faculty = _f.AddFaculty();
        var own = _f.AddDepartment(faculty);
        _f.Evaluator.Grant(Permissions.Faculty.Read, ResourceScope.Department(own.Id));

        var result = await new GetFaculty.Handler(_f.CreateAccess(), _f.Faculties)
            .Handle(new GetFaculty.Query(faculty.Id), default);

        Assert.Equal(403, result.StatusCode);
    }

    [Fact]
    public async Task Faculty_CannotBeDeactivated_WhileItHasActiveDepartments()
    {
        var faculty = _f.AddFaculty();
        _f.AddDepartment(faculty);
        _f.Evaluator.Grant(Permissions.Faculty.Deactivate, ResourceScope.University);

        var handler = new SetFacultyStatus.Handler(
            _f.CreateAccess(), _f.Faculties, _f.Departments, _f.UnitOfWork, _f.Audit);
        var result = await handler.Handle(new SetFacultyStatus.Command(faculty.Id, false), default);

        Assert.Equal(409, result.StatusCode);
        Assert.Equal("Faculty.HasActiveDepartments", result.Errors.Single().Code);
        Assert.True(faculty.IsActive);
    }

    [Fact]
    public async Task CreateFaculty_WithDuplicateCode_ReturnsConflict()
    {
        _f.AddFaculty("ENG");
        _f.Evaluator.Grant(Permissions.Faculty.Create, ResourceScope.University);

        var result = await CreateFacultyHandler().Handle(new CreateFaculty.Command(" eng ", "Baska"), default);

        Assert.Equal(409, result.StatusCode);
        Assert.Equal("Faculty.CodeAlreadyExists", result.Errors.Single().Code);
        Assert.Single(_f.Faculties.Items);
    }

    [Fact]
    public async Task CreateDepartment_InInactiveFaculty_ReturnsConflict()
    {
        var faculty = _f.AddFaculty();
        faculty.Deactivate(DateTime.UtcNow);
        _f.Evaluator.Grant(Permissions.Department.Create, ResourceScope.University);

        var result = await CreateDeptHandler().Handle(new CreateDepartment.Command(faculty.Id, "NEW", "Yeni"), default);

        Assert.Equal(409, result.StatusCode);
        Assert.Empty(_f.Departments.Items);
    }
}
