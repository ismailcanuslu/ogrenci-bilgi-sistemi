using BuildingBlocks.Application.Abstractions.Security;

namespace Security.Infrastructure.Authorization;

/// <summary>
/// Sistem tarafından seed edilen rol tanımları. Üniversiteye özgü politikalar burada
/// sabitlenmez; roller yönetilebilir ve yeni roller API üzerinden oluşturulabilir.
/// </summary>
public static class DefaultRoles
{
    public const string SystemAdministrator = "System Administrator";
    public const string StudentAffairs = "Student Affairs";
    public const string FacultyManager = "Faculty Manager";
    public const string DepartmentManager = "Department Manager";
    public const string Instructor = "Instructor";
    public const string Student = "Student";

    public static IReadOnlyDictionary<string, string[]> Definitions { get; } =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            // Yalnızca yetkilendirme yönetimi; akademik operasyon yetkisi içermez.
            [SystemAdministrator] =
            [
                Permissions.Authorization.RoleManage,
                Permissions.Authorization.PermissionManage,
                Permissions.Authorization.PatCreate,
                Permissions.Authorization.PatRevoke,
                Permissions.Authorization.PatRead
            ],

            // Akademik operasyonlar; rol/permission yönetimi içermez.
            [StudentAffairs] =
            [
                Permissions.Faculty.Read, Permissions.Faculty.Create,
                Permissions.Faculty.Update, Permissions.Faculty.Deactivate,
                Permissions.Department.Read, Permissions.Department.Create,
                Permissions.Department.Update, Permissions.Department.Deactivate,
                Permissions.AcademicTerm.Manage,
                Permissions.Curriculum.Read, Permissions.Curriculum.Create,
                Permissions.Curriculum.Update, Permissions.Curriculum.Publish,
                Permissions.Student.Read, Permissions.Student.Create,
                Permissions.Student.Update, Permissions.Student.EnrollmentManage,
                Permissions.Course.Read, Permissions.Course.Create,
                Permissions.Course.Update, Permissions.Course.AssignmentManage,
                Permissions.Course.ScheduleManage,
                Permissions.CourseSelection.Read, Permissions.CourseSelection.Manage,
                Permissions.CourseSelection.EctsLimitOverride,
                Permissions.Authorization.PatCreate,
                Permissions.Authorization.PatRevoke,
                Permissions.Authorization.PatRead
            ],

            [FacultyManager] =
            [
                Permissions.Faculty.Read, Permissions.Faculty.Update,
                Permissions.Department.Read, Permissions.Department.Create,
                Permissions.Department.Update, Permissions.Department.Deactivate,
                Permissions.Curriculum.Read, Permissions.Curriculum.Update
            ],

            [DepartmentManager] =
            [
                Permissions.Department.Read, Permissions.Department.Update,
                Permissions.Curriculum.Read, Permissions.Curriculum.Create,
                Permissions.Curriculum.Update
            ],

            [Instructor] =
            [
                Permissions.Course.Read, Permissions.Course.Update,
                Permissions.Student.Read
            ],

            [Student] =
            [
                Permissions.Course.Read, Permissions.CourseSelection.Read,
                Permissions.Student.Read
            ]
        };
}
