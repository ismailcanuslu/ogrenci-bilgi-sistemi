namespace BuildingBlocks.Application.Abstractions.Security;

/// <summary>
/// OBS permission kataloğu. Roller bu izinleri string olarak saklar; yeni bir özellik
/// geliştirildikçe ilgili izinler kullanılmaya başlanır.
/// </summary>
public static class Permissions
{
    public static class Faculty
    {
        public const string Read = "faculty.read";
        public const string Create = "faculty.create";
        public const string Update = "faculty.update";
        public const string Deactivate = "faculty.deactivate";
    }

    public static class Department
    {
        public const string Read = "department.read";
        public const string Create = "department.create";
        public const string Update = "department.update";
        public const string Deactivate = "department.deactivate";
    }

    public static class Curriculum
    {
        public const string Read = "curriculum.read";
        public const string Create = "curriculum.create";
        public const string Update = "curriculum.update";
        public const string Publish = "curriculum.publish";
    }

    public static class Student
    {
        public const string Read = "student.read";
        public const string Create = "student.create";
        public const string Update = "student.update";
        public const string EnrollmentManage = "student.enrollment.manage";
    }

    public static class Course
    {
        public const string Read = "course.read";
        public const string Create = "course.create";
        public const string Update = "course.update";
        public const string AssignmentManage = "course.assignment.manage";
        public const string ScheduleManage = "course.schedule.manage";
        public const string AssessmentManage = "course.assessment.manage";
        public const string BellCurveManage = "course.grading.bell-curve.manage";
    }

    public static class CourseSelection
    {
        public const string Read = "course-selection.read";
        public const string Manage = "course-selection.manage";
        public const string EctsLimitOverride = "course-selection.ects-limit.override";
    }

    public static class ExamTemplate
    {
        public const string Read = "exam-template.read";
        public const string Create = "exam-template.create";
        public const string Update = "exam-template.update";
    }

    public static class AcademicTerm
    {
        public const string Manage = "academic-term.manage";
    }

    public static class Authorization
    {
        public const string RoleManage = "authorization.role.manage";
        public const string PermissionManage = "authorization.permission.manage";
        public const string PatCreate = "authorization.pat.create";
        public const string PatRevoke = "authorization.pat.revoke";
        public const string PatRead = "authorization.pat.read";
    }

    public static class CampusLife
    {
        public const string ReservationCreate = "campus-life.reservation.create";
        public const string ReservationManage = "campus-life.reservation.manage";
        public const string ClubManage = "campus-life.club.manage";
        public const string EventManage = "campus-life.event.manage";
    }

    public static class SupportDesk
    {
        public const string TicketCreate = "support-desk.ticket.create";
        public const string TicketManage = "support-desk.ticket.manage";
    }

    public static class ContinuingEducation
    {
        public const string ProgramRead = "continuing-education.program.read";
        public const string ProgramManage = "continuing-education.program.manage";
        public const string AttendanceManage = "continuing-education.attendance.manage";
        public const string GradeManage = "continuing-education.grade.manage";
    }

    public static class DocumentManagement
    {
        public const string DocumentCreate = "document.create";
        public const string DocumentManage = "document.manage";
        public const string DocumentSign = "document.sign";
        public const string DocumentVerify = "document.verify";
    }

    public static IReadOnlyCollection<string> All { get; } =
    [
        Faculty.Read, Faculty.Create, Faculty.Update, Faculty.Deactivate,
        Department.Read, Department.Create, Department.Update, Department.Deactivate,
        Curriculum.Read, Curriculum.Create, Curriculum.Update, Curriculum.Publish,
        Student.Read, Student.Create, Student.Update, Student.EnrollmentManage,
        Course.Read, Course.Create, Course.Update, Course.AssignmentManage,
        Course.ScheduleManage, Course.AssessmentManage, Course.BellCurveManage,
        CourseSelection.Read, CourseSelection.Manage, CourseSelection.EctsLimitOverride,
        ExamTemplate.Read, ExamTemplate.Create, ExamTemplate.Update,
        AcademicTerm.Manage,
        Authorization.RoleManage, Authorization.PermissionManage,
        Authorization.PatCreate, Authorization.PatRevoke, Authorization.PatRead,
        CampusLife.ReservationCreate, CampusLife.ReservationManage, CampusLife.ClubManage, CampusLife.EventManage,
        SupportDesk.TicketCreate, SupportDesk.TicketManage,
        ContinuingEducation.ProgramRead, ContinuingEducation.ProgramManage, ContinuingEducation.AttendanceManage, ContinuingEducation.GradeManage,
        DocumentManagement.DocumentCreate, DocumentManagement.DocumentManage, DocumentManagement.DocumentSign, DocumentManagement.DocumentVerify
    ];
}
