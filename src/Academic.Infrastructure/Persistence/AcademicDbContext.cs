using Academic.Application.Abstractions;
using Academic.Domain.AcademicYears;
using Academic.Domain.Curriculums;
using Academic.Domain.Departments;
using Academic.Domain.Faculties;
using Academic.Domain.Programs;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence;

public sealed class AcademicDbContext(DbContextOptions<AcademicDbContext> options)
    : DbContext(options), IAcademicUnitOfWork
{
    public DbSet<Faculty> Faculties => Set<Faculty>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();

    public DbSet<AcademicTerm> AcademicTerms => Set<AcademicTerm>();

    public DbSet<Program> Programs => Set<Program>();

    public DbSet<Curriculum> Curriculums => Set<Curriculum>();

    public DbSet<CurriculumVersion> CurriculumVersions => Set<CurriculumVersion>();

    public DbSet<CurriculumCourse> CurriculumCourses => Set<CurriculumCourse>();

    public DbSet<Academic.Domain.Students.Student> Students => Set<Academic.Domain.Students.Student>();

    public DbSet<Academic.Domain.Courses.Course> Courses => Set<Academic.Domain.Courses.Course>();

    public DbSet<Academic.Domain.Courses.CourseOffering> CourseOfferings => Set<Academic.Domain.Courses.CourseOffering>();

    public DbSet<Academic.Domain.Courses.CourseOfferingInstructor> CourseOfferingInstructors => Set<Academic.Domain.Courses.CourseOfferingInstructor>();

    public DbSet<Academic.Domain.Courses.CourseOfferingSchedule> CourseOfferingSchedules => Set<Academic.Domain.Courses.CourseOfferingSchedule>();

    public DbSet<Academic.Domain.Students.StudentProgramEnrollment> StudentProgramEnrollments => Set<Academic.Domain.Students.StudentProgramEnrollment>();

    public DbSet<Academic.Domain.Instructors.InstructorDepartmentAffiliation> InstructorDepartmentAffiliations => Set<Academic.Domain.Instructors.InstructorDepartmentAffiliation>();

    public DbSet<Academic.Domain.Students.StudentTuition> StudentTuitions => Set<Academic.Domain.Students.StudentTuition>();

    public DbSet<Academic.Domain.Students.StudentMilitaryStatus> StudentMilitaryStatuses => Set<Academic.Domain.Students.StudentMilitaryStatus>();

    public DbSet<Academic.Domain.Students.StudentDisciplinaryRecord> StudentDisciplinaryRecords => Set<Academic.Domain.Students.StudentDisciplinaryRecord>();

    public DbSet<Academic.Domain.Students.StudentBankDetails> StudentBankDetails => Set<Academic.Domain.Students.StudentBankDetails>();

    public DbSet<Academic.Domain.Students.StudentStudyExtension> StudentStudyExtensions => Set<Academic.Domain.Students.StudentStudyExtension>();

    public DbSet<Academic.Domain.Students.StudentHistoryRecord> StudentHistoryRecords => Set<Academic.Domain.Students.StudentHistoryRecord>();

    public DbSet<Academic.Domain.Students.StudentProfilePicture> StudentProfilePictures => Set<Academic.Domain.Students.StudentProfilePicture>();

    public DbSet<Academic.Domain.Students.StudentAlumniInfo> StudentAlumniInfos => Set<Academic.Domain.Students.StudentAlumniInfo>();

    public DbSet<Academic.Domain.Students.StudentAdvisorAssignment> StudentAdvisorAssignments => Set<Academic.Domain.Students.StudentAdvisorAssignment>();

    public DbSet<Academic.Domain.Courses.CourseOfferingLiveSession> CourseOfferingLiveSessions => Set<Academic.Domain.Courses.CourseOfferingLiveSession>();

    public DbSet<Academic.Domain.AcademicCalendars.AcademicCalendarEvent> AcademicCalendarEvents => Set<Academic.Domain.AcademicCalendars.AcademicCalendarEvent>();

    public DbSet<Academic.Domain.Dashboard.UserDashboardWidget> UserDashboardWidgets => Set<Academic.Domain.Dashboard.UserDashboardWidget>();

    public DbSet<Academic.Domain.Dashboard.UserPersonalShortcut> UserPersonalShortcuts => Set<Academic.Domain.Dashboard.UserPersonalShortcut>();

    public DbSet<Academic.Domain.Programs.ProgramProfile> ProgramProfiles => Set<Academic.Domain.Programs.ProgramProfile>();

    public DbSet<Academic.Domain.Courses.CourseSyllabus> CourseSyllabi => Set<Academic.Domain.Courses.CourseSyllabus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("academic");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademicDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
