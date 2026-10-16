using Academic.Domain.Departments;
using Academic.Domain.Faculties;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Domain.Authorization;

namespace Academic.Application.Abstractions;

public sealed record ListAccess(
    Error? Denied,
    bool Unrestricted,
    IReadOnlyCollection<Guid> FacultyIds,
    IReadOnlyCollection<Guid> DepartmentIds);

/// <summary>
/// Fakülte/bölüm kaynakları için permission + kapsam denetiminin tek uygulaması.
/// Handler'lar kapsam kurallarını kendileri yazmaz.
/// </summary>
public sealed class AcademicAccess(
    IPermissionGuard guard,
    ICurrentUser currentUser,
    IPermissionEvaluator evaluator)
{
    /// <summary>Üniversite genelinde bir işlem (ör. fakülte oluşturma).</summary>
    public Task<Error?> CheckUniversityAsync(string permission, CancellationToken ct)
        => guard.CheckAsync(permission, [ResourceScope.University], ct);

    /// <summary>
    /// Fakülte kapsamında kontrol. Kaynak yoksa yalnızca üniversite geneli yetkili olanlar
    /// "bulunamadı" görür; diğerleri Forbidden alır (varlık sızdırılmaz).
    /// </summary>
    public Task<Error?> CheckFacultyAsync(string permission, Guid facultyId, bool exists, CancellationToken ct)
        => guard.CheckAsync(
            permission,
            exists ? [ResourceScope.Faculty(facultyId)] : [],
            ct);

    public Task<Error?> CheckDepartmentAsync(string permission, Department? department, CancellationToken ct)
        => guard.CheckAsync(
            permission,
            department is null
                ? []
                : [ResourceScope.Department(department.Id), ResourceScope.Faculty(department.FacultyId)],
            ct);

    /// <summary>Yeni bölüm oluşturma: hedef fakülte kapsamında create izni gerekir.</summary>
    public Task<Error?> CheckDepartmentCreationAsync(
        string permission,
        Guid facultyId,
        CancellationToken ct)
        => guard.CheckAsync(permission, [ResourceScope.Faculty(facultyId)], ct);

    public async Task<ListAccess> GetListAccessAsync(string permission, CancellationToken ct)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return new ListAccess(
                new Error(PermissionGuard.UnauthenticatedCode, ErrorType.Unauthorized), false, [], []);
        }

        var grants = await evaluator.GetGrantedScopesAsync(userId, permission, ct);

        if (grants.Count == 0)
        {
            return new ListAccess(
                new Error(PermissionGuard.ForbiddenCode, ErrorType.Forbidden), false, [], []);
        }

        return new ListAccess(
            null,
            grants.Any(g => g.Type == ScopeType.University),
            grants.Where(g => g.Type == ScopeType.Faculty && g.Id is not null).Select(g => g.Id!.Value).ToArray(),
            grants.Where(g => g.Type == ScopeType.Department && g.Id is not null).Select(g => g.Id!.Value).ToArray());
    }

    public Task<Error?> CheckFacultyAsync(string permission, Faculty? faculty, CancellationToken ct)
        => CheckFacultyAsync(permission, faculty?.Id ?? Guid.Empty, faculty is not null, ct);

    /// <summary>
    /// Akademik dönem işlemleri üniversite geneli yetki ister (academic-term.manage).
    /// </summary>
    public Task<Error?> CheckAcademicTermManageAsync(CancellationToken ct)
        => guard.CheckAsync(Permissions.AcademicTerm.Manage, [ResourceScope.University], ct);

    /// <summary>
    /// Program üzerinde işlem (okuma / güncelleme). İlgili programın bağlı olduğu departman veya fakülte kapsamında denetlenir.
    /// </summary>
    public Task<Error?> CheckProgramAsync(
        string permission,
        Guid? departmentId,
        Guid? facultyId,
        bool exists,
        CancellationToken ct)
    {
        if (!exists || departmentId is null || facultyId is null)
        {
            return guard.CheckAsync(permission, [], ct);
        }

        return guard.CheckAsync(
            permission,
            [ResourceScope.Department(departmentId.Value), ResourceScope.Faculty(facultyId.Value)],
            ct);
    }

    /// <summary>
    /// Yeni program oluşturma: departmanın bağlı olduğu departman veya fakülte kapsamında yetki ister.
    /// </summary>
    public Task<Error?> CheckProgramCreationAsync(
        string permission,
        Guid departmentId,
        Guid facultyId,
        CancellationToken ct)
        => guard.CheckAsync(
            permission,
            [ResourceScope.Department(departmentId), ResourceScope.Faculty(facultyId)],
            ct);

    /// <summary>
    /// Müfredat üzerinde işlem: ilgili müfredatın programının bağlı olduğu departman veya fakülte kapsamında denetlenir.
    /// </summary>
    public Task<Error?> CheckCurriculumAsync(
        string permission,
        Guid? departmentId,
        Guid? facultyId,
        bool exists,
        CancellationToken ct)
    {
        if (!exists || departmentId is null || facultyId is null)
        {
            return guard.CheckAsync(permission, [], ct);
        }

        return guard.CheckAsync(
            permission,
            [ResourceScope.Department(departmentId.Value), ResourceScope.Faculty(facultyId.Value)],
            ct);
    }

    /// <summary>
    /// Yeni müfredat oluşturma: hedef programın bağlı olduğu departman veya fakülte kapsamında curriculum.create yetkisi ister.
    /// </summary>
    public Task<Error?> CheckCurriculumCreationAsync(
        string permission,
        Guid departmentId,
        Guid facultyId,
        CancellationToken ct)
        => guard.CheckAsync(
            permission,
            [ResourceScope.Department(departmentId), ResourceScope.Faculty(facultyId)],
            ct);

    /// <summary>
    /// Öğrenci görüntüleme / güncelleme işlemi. Öğrenci kendi kaydını okuyabilir (Self), personel ise bölüm/fakülte kapsamında erişir.
    /// </summary>
    public Task<Error?> CheckStudentAsync(
        string permission,
        Guid? studentUserId,
        Guid? departmentId,
        Guid? facultyId,
        bool exists,
        CancellationToken ct)
    {
        if (!exists || departmentId is null || facultyId is null)
        {
            return guard.CheckAsync(permission, [], ct);
        }

        List<ResourceScope> scopes = [ResourceScope.Department(departmentId.Value), ResourceScope.Faculty(facultyId.Value)];
        if (studentUserId.HasValue && studentUserId.Value != Guid.Empty)
        {
            scopes.Add(ResourceScope.Self(studentUserId.Value));
        }

        return guard.CheckAsync(permission, scopes, ct);
    }

    /// <summary>
    /// Manuel öğrenci oluşturma: öğrencinin atanacağı departman veya fakülte kapsamında yetki denetimi.
    /// </summary>
    public Task<Error?> CheckStudentCreationAsync(
        string permission,
        Guid departmentId,
        Guid facultyId,
        CancellationToken ct)
        => guard.CheckAsync(
            permission,
            [ResourceScope.Department(departmentId), ResourceScope.Faculty(facultyId)],
            ct);

    /// <summary>
    /// Ders kataloğu üzerinde işlem (okuma / güncelleme / durum değiştirme).
    /// </summary>
    public Task<Error?> CheckCourseAsync(
        string permission,
        Guid? departmentId,
        Guid? facultyId,
        bool exists,
        CancellationToken ct)
    {
        if (!exists || departmentId is null || facultyId is null)
        {
            return guard.CheckAsync(permission, [], ct);
        }

        return guard.CheckAsync(
            permission,
            [ResourceScope.Department(departmentId.Value), ResourceScope.Faculty(facultyId.Value)],
            ct);
    }

    /// <summary>
    /// Yeni katalog dersi oluşturma: ilgili departman veya fakülte kapsamında yetki denetimi.
    /// </summary>
    public Task<Error?> CheckCourseCreationAsync(
        string permission,
        Guid departmentId,
        Guid facultyId,
        CancellationToken ct)
        => guard.CheckAsync(
            permission,
            [ResourceScope.Department(departmentId), ResourceScope.Faculty(facultyId)],
            ct);

    /// <summary>
    /// Dönemlik ders açımı üzerinde işlem (görevlendirme, program, şube).
    /// Öğretim elemanı görevlendirildiği ders açımında (Course scope) işlem yapabilir.
    /// </summary>
    public Task<Error?> CheckCourseOfferingAsync(
        string permission,
        Guid offeringId,
        bool hasAssignedInstructors,
        Guid? departmentId,
        Guid? facultyId,
        bool exists,
        CancellationToken ct)
    {
        if (!exists || departmentId is null || facultyId is null)
        {
            return guard.CheckAsync(permission, [], ct);
        }

        List<ResourceScope> scopes =
        [
            ResourceScope.Department(departmentId.Value),
            ResourceScope.Faculty(facultyId.Value)
        ];

        if (hasAssignedInstructors)
        {
            scopes.Add(ResourceScope.Course(offeringId));
        }

        return guard.CheckAsync(permission, scopes, ct);
    }

    public Task<Error?> CheckCourseOfferingAsync(
        string permission,
        Guid offeringId,
        Guid? instructorId,
        Guid? departmentId,
        Guid? facultyId,
        bool exists,
        CancellationToken ct)
        => CheckCourseOfferingAsync(
            permission,
            offeringId,
            instructorId.HasValue && instructorId.Value != Guid.Empty,
            departmentId,
            facultyId,
            exists,
            ct);
}

