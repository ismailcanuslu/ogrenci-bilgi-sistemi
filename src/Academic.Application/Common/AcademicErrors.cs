using BuildingBlocks.Application.Results;

namespace Academic.Application.Common;

public static class AcademicErrors
{
    public static class Faculty
    {
        public static Error NotFound(string? path = null) => new("Faculty.NotFound", ErrorType.NotFound, path);
        public static Error CodeAlreadyExists => new("Faculty.CodeAlreadyExists", ErrorType.Conflict, "code");
        public static Error HasActiveDepartments => new("Faculty.HasActiveDepartments", ErrorType.Conflict);
        public static Error Inactive => new("Faculty.Inactive", ErrorType.Conflict);
    }

    public static class Department
    {
        public static Error NotFound(string? path = null) => new("Department.NotFound", ErrorType.NotFound, path);
        public static Error CodeAlreadyExists => new("Department.CodeAlreadyExists", ErrorType.Conflict, "code");
    }

    public static class AcademicYear
    {
        public static Error NotFound(string? path = null) => new("AcademicYear.NotFound", ErrorType.NotFound, path);
        public static Error CodeAlreadyExists => new("AcademicYear.CodeAlreadyExists", ErrorType.Conflict, "code");
        public static Error Inactive => new("AcademicYear.Inactive", ErrorType.Conflict);
    }

    public static class AcademicTerm
    {
        public static Error NotFound(string? path = null) => new("AcademicTerm.NotFound", ErrorType.NotFound, path);
        public static Error TermTypeAlreadyExistsInYear => new("AcademicTerm.TermTypeAlreadyExistsInYear", ErrorType.Conflict);
    }

    public static class Program
    {
        public static Error NotFound(string? path = null) => new("Program.NotFound", ErrorType.NotFound, path);
        public static Error CodeAlreadyExists => new("Program.CodeAlreadyExists", ErrorType.Conflict, "code");
        public static Error Inactive => new("Program.Inactive", ErrorType.Conflict);
    }

    public static class Curriculum
    {
        public static Error NotFound(string? path = null) => new("Curriculum.NotFound", ErrorType.NotFound, path);
        public static Error AlreadyExistsForAcademicYear => new("Curriculum.AlreadyExistsForAcademicYear", ErrorType.Conflict);
        public static Error VersionNotFound => new("Curriculum.VersionNotFound", ErrorType.NotFound);
        public static Error VersionNotDraft => new("Curriculum.VersionNotDraft", ErrorType.Conflict);
        public static Error Inactive => new("Curriculum.Inactive", ErrorType.Conflict);
    }

    public static class Student
    {
        public static Error NotFound(string? path = null) => new("Student.NotFound", ErrorType.NotFound, path);
        public static Error StudentNumberConflict => new("Student.StudentNumberConflict", ErrorType.Conflict, "studentNumber");
        public static Error EmailConflict => new("Student.EmailConflict", ErrorType.Conflict, "email");
        public static Error PublishedCurriculumRequired => new("Curriculum.PublishedVersionRequired", ErrorType.Validation);
    }

    public static class Course
    {
        public static Error NotFound(string? path = null) => new("Course.NotFound", ErrorType.NotFound, path);
        public static Error CodeConflict => new("Course.CodeConflict", ErrorType.Conflict, "code");
        public static Error Inactive => new("Course.Inactive", ErrorType.Conflict);
    }

    public static class CourseOffering
    {
        public static Error NotFound(string? path = null) => new("CourseOffering.NotFound", ErrorType.NotFound, path);
        public static Error SectionConflict => new("CourseOffering.SectionConflict", ErrorType.Conflict, "section");
        public static Error ClassroomConflict => new("CourseOffering.ClassroomConflict", ErrorType.Conflict);
        public static Error InstructorScheduleConflict => new("CourseOffering.InstructorScheduleConflict", ErrorType.Conflict);
    }
}

