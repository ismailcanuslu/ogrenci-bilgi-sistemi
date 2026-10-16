using Academic.Application.Features.AcademicYears;
using Academic.Application.Features.CourseOfferings;
using Academic.Application.Features.Courses;
using Academic.Application.Features.Curriculums;
using Academic.Application.Features.Departments;
using Academic.Application.Features.Faculties;
using Academic.Application.Features.Instructors;
using Academic.Application.Features.Programs;
using Academic.Application.Features.Students;
using Academic.Domain.AcademicYears;
using Academic.Domain.Courses;
using Academic.Domain.Curriculums;
using Academic.Domain.Instructors;
using Academic.Domain.Programs;
using Academic.Domain.Students;
using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Academic.Api.Endpoints;

public sealed record FacultyRequest(string Code, string Name);

public sealed record DepartmentRequest(string Code, string Name);

public sealed record CreateDepartmentRequest(string Code, string Name);

public sealed record StatusRequest(bool IsActive);

public sealed record AcademicYearRequest(string Code, string Name, DateOnly StartDate, DateOnly EndDate);

public sealed record AddTermRequest(TermType TermType, string Name, DateOnly StartDate, DateOnly EndDate, bool IsRegistrationOpen);

public sealed record SetTermRegistrationRequest(bool IsRegistrationOpen);

public sealed record ProgramRequest(string Code, string Name, DegreeLevel DegreeLevel);

public sealed record CreateCurriculumRequest(Guid ProgramId, Guid AcademicYearId, string Code, string Name);

public sealed record CreateCurriculumVersionRequest(string? Notes);

public sealed record AddCurriculumCourseRequest(
    string CourseCode,
    string CourseName,
    int Ects,
    int Credit,
    int Semester,
    bool IsElective,
    CourseCategory Category,
    string? Prerequisites);

public sealed record CreateStudentRequest(
    string StudentNumber,
    string FirstName,
    string LastName,
    string Email,
    Guid DepartmentId,
    Guid ProgramId,
    Guid CurriculumVersionId,
    int EnrollmentYear,
    DateTime EnrollmentDateUtc,
    Guid? UserId);

public sealed record UpdateStudentRequest(string FirstName, string LastName, string Email);

public sealed record ChangeStudentStatusRequest(StudentStatus Status, string? Reason);

public sealed record EnrollStudentInProgramRequest(
    Guid DepartmentId,
    Guid ProgramId,
    Guid CurriculumVersionId,
    EnrollmentType EnrollmentType,
    int EnrollmentYear,
    bool IsPrimary,
    string? Notes);

public sealed record AssignInstructorDepartmentRequest(
    Guid FacultyId,
    Guid DepartmentId,
    AffiliationType AffiliationType,
    string? Title,
    bool IsPrimary,
    DateTime StartDateUtc,
    DateTime? EndDateUtc);

public sealed record CreateCourseRequest(
    Guid DepartmentId,
    string Code,
    string Name,
    string? Description,
    int DefaultEcts,
    int DefaultCredit,
    string? Prerequisites);

public sealed record UpdateCourseRequest(
    string Name,
    string? Description,
    int DefaultEcts,
    int DefaultCredit,
    string? Prerequisites);

public sealed record CreateCourseOfferingRequest(
    Guid CourseId,
    Guid AcademicTermId,
    int Section,
    int Capacity,
    Guid? PrimaryInstructorId);

public sealed record AssignInstructorRequest(
    Guid InstructorId,
    bool IsPrimary = true,
    string? RoleOrTitle = null);

public sealed record AddOfferingScheduleRequest(
    SessionType SessionType,
    string BuildingOrFaculty,
    string Classroom,
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    Guid? InstructorId,
    bool IsOnline);

public sealed record UpdateOfferingCapacityRequest(int Capacity);


public static class AcademicEndpoints
{
    public static IEndpointRouteBuilder MapAcademicEndpoints(this IEndpointRouteBuilder app)
    {
        // Tüm akademik endpoint'ler kimlik doğrulaması ister; permission + kapsam denetimi
        // handler'larda (application katmanı) yapılır.
        var group = app.MapVersionedGroup("academic", 1, 0)
            .WithTags("Academic")
            .RequireAuthorization();

        MapFaculties(group);
        MapDepartments(group);
        MapAcademicYears(group);
        MapPrograms(group);
        MapCurriculums(group);
        MapStudents(group);
        MapInstructors(group);
        MapCourses(group);
        MapCourseOfferings(group);

        return app;
    }

    private static void MapStudents(RouteGroupBuilder group)
    {
        var students = group.MapGroup("/students");

        students.MapGet("/", async (
            [AsParameters] ListStudentsQuery query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListStudents")
            .WithSummary("Öğrencileri listeler (yetki kapsamıyla sınırlı)");

        students.MapGet("/{id:guid}", async (
            Guid id, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentQuery(id), ct)).ToHttpResult(http))
            .WithName("GetStudent")
            .WithSummary("Öğrenci detayı");

        students.MapPost("/", async (
            CreateStudentRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateStudentCommand(
                body.StudentNumber,
                body.FirstName,
                body.LastName,
                body.Email,
                body.DepartmentId,
                body.ProgramId,
                body.CurriculumVersionId,
                body.EnrollmentYear,
                body.EnrollmentDateUtc,
                body.UserId), ct)).ToHttpResult(http))
            .WithName("CreateStudent")
            .WithSummary("Manuel öğrenci kaydı oluşturur (student.create)");

        students.MapPut("/{id:guid}", async (
            Guid id, UpdateStudentRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new UpdateStudentCommand(id, body.FirstName, body.LastName, body.Email), ct)).ToHttpResult(http))
            .WithName("UpdateStudent")
            .WithSummary("Öğrenci bilgilerini günceller (student.update)");

        students.MapPut("/{id:guid}/status", async (
            Guid id, ChangeStudentStatusRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ChangeStudentStatusCommand(id, body.Status, body.Reason), ct)).ToHttpResult(http))
            .WithName("ChangeStudentStatus")
            .WithSummary("Öğrenci durumunu değiştirir (student.enrollment.manage)");

        students.MapPost("/{id:guid}/programs", async (
            Guid id, EnrollStudentInProgramRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new EnrollStudentInProgramCommand(
                id,
                body.DepartmentId,
                body.ProgramId,
                body.CurriculumVersionId,
                body.EnrollmentType,
                body.EnrollmentYear,
                body.IsPrimary,
                body.Notes), ct)).ToHttpResult(http))
            .WithName("EnrollStudentInProgram")
            .WithSummary("Öğrenciye ÇAP, Yandal veya ek program kaydı ekler (student.update)");

        students.MapPut("/{id:guid}/programs/{programEnrollmentId:guid}/primary", async (
            Guid id, Guid programEnrollmentId, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new SetStudentPrimaryProgramCommand(id, programEnrollmentId), ct)).ToHttpResult(http))
            .WithName("SetStudentPrimaryProgram")
            .WithSummary("Öğrencinin aktif/birincil akademik programını seçer (öğrenci program değiştirici)");
    }

    private static void MapInstructors(RouteGroupBuilder group)
    {
        var instructors = group.MapGroup("/instructors");

        instructors.MapPost("/{instructorId:guid}/affiliations", async (
            Guid instructorId, AssignInstructorDepartmentRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new AssignInstructorDepartmentCommand(
                instructorId,
                body.FacultyId,
                body.DepartmentId,
                body.AffiliationType,
                body.Title,
                body.IsPrimary,
                body.StartDateUtc,
                body.EndDateUtc), ct)).ToHttpResult(http))
            .WithName("AssignInstructorDepartment")
            .WithSummary("Öğretim elemanına asil kadro veya ek görevlendirme bölümü atar (course.assignment.manage)");

        instructors.MapGet("/{instructorId:guid}/affiliations", async (
            Guid instructorId, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ListInstructorAffiliationsQuery(instructorId), ct)).ToHttpResult(http))
            .WithName("ListInstructorAffiliations")
            .WithSummary("Öğretim elemanının bağlı olduğu tüm bölümleri ve görevlendirmeleri listeler");
    }

    private static void MapCourses(RouteGroupBuilder group)
    {
        var courses = group.MapGroup("/courses");

        courses.MapGet("/", async (
            [AsParameters] ListCoursesQuery query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListCourses")
            .WithSummary("Katalog derslerini listeler");

        courses.MapGet("/{id:guid}", async (
            Guid id, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetCourseQuery(id), ct)).ToHttpResult(http))
            .WithName("GetCourse")
            .WithSummary("Katalog dersi detayı");

        courses.MapPost("/", async (
            CreateCourseRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateCourseCommand(
                body.DepartmentId,
                body.Code,
                body.Name,
                body.Description,
                body.DefaultEcts,
                body.DefaultCredit,
                body.Prerequisites), ct)).ToHttpResult(http))
            .WithName("CreateCourse")
            .WithSummary("Katalog dersi oluşturur (course.create)");

        courses.MapPut("/{id:guid}", async (
            Guid id, UpdateCourseRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new UpdateCourseCommand(
                id,
                body.Name,
                body.Description,
                body.DefaultEcts,
                body.DefaultCredit,
                body.Prerequisites), ct)).ToHttpResult(http))
            .WithName("UpdateCourse")
            .WithSummary("Katalog dersini günceller (course.update)");

        courses.MapPut("/{id:guid}/status", async (
            Guid id, StatusRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new SetCourseStatusCommand(id, body.IsActive), ct)).ToHttpResult(http))
            .WithName("SetCourseStatus")
            .WithSummary("Katalog dersini etkinleştirir/pasifleştirir (course.update)");
    }

    private static void MapCourseOfferings(RouteGroupBuilder group)
    {
        var offerings = group.MapGroup("/course-offerings");

        offerings.MapGet("/", async (
            [AsParameters] ListCourseOfferingsQuery query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListCourseOfferings")
            .WithSummary("Dönemlik ders açımlarını listeler");

        offerings.MapGet("/{id:guid}", async (
            Guid id, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetCourseOfferingQuery(id), ct)).ToHttpResult(http))
            .WithName("GetCourseOffering")
            .WithSummary("Dönemlik ders açımı ve program detayı");

        offerings.MapPost("/", async (
            CreateCourseOfferingRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateCourseOfferingCommand(
                body.CourseId,
                body.AcademicTermId,
                body.Section,
                body.Capacity,
                body.PrimaryInstructorId), ct)).ToHttpResult(http))
            .WithName("CreateCourseOffering")
            .WithSummary("Dönemlik ders açımı oluşturur (course.create)");

        offerings.MapPut("/{id:guid}/instructor", async (
            Guid id, AssignInstructorRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new AssignInstructorCommand(id, body.InstructorId, body.IsPrimary, body.RoleOrTitle), ct)).ToHttpResult(http))
            .WithName("AssignInstructor")
            .WithSummary("Ders açımına öğretim elemanı atar veya rolünü günceller (course.assignment.manage)");

        offerings.MapDelete("/{id:guid}/instructors/{instructorId:guid}", async (
            Guid id, Guid instructorId, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new RemoveInstructorCommand(id, instructorId), ct)).ToHttpResult(http))
            .WithName("RemoveInstructor")
            .WithSummary("Ders açımından öğretim elemanını kaldırır (course.assignment.manage)");

        offerings.MapPost("/{id:guid}/schedules", async (
            Guid id, AddOfferingScheduleRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new AddOfferingScheduleCommand(
                id,
                body.SessionType,
                body.BuildingOrFaculty,
                body.Classroom,
                body.DayOfWeek,
                body.StartTime,
                body.EndTime,
                body.InstructorId,
                body.IsOnline), ct)).ToHttpResult(http))
            .WithName("AddOfferingSchedule")
            .WithSummary("Ders açımına haftalık program saati ekler (course.schedule.manage)");

        offerings.MapDelete("/{id:guid}/schedules/{scheduleId:guid}", async (
            Guid id, Guid scheduleId, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new RemoveOfferingScheduleCommand(id, scheduleId), ct)).ToHttpResult(http))
            .WithName("RemoveOfferingSchedule")
            .WithSummary("Haftalık program saatini siler (course.schedule.manage)");

        offerings.MapPut("/{id:guid}/capacity", async (
            Guid id, UpdateOfferingCapacityRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new UpdateOfferingCapacityCommand(id, body.Capacity), ct)).ToHttpResult(http))
            .WithName("UpdateOfferingCapacity")
            .WithSummary("Kontenjan günceller (course.assignment.manage)");

        offerings.MapPut("/{id:guid}/registration-status", async (
            Guid id, StatusRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new SetOfferingRegistrationStatusCommand(id, body.IsActive), ct)).ToHttpResult(http))
            .WithName("SetOfferingRegistrationStatus")
            .WithSummary("Ders açımının kayıt durumunu açar/kapatır (course.assignment.manage)");
    }

    private static void MapFaculties(RouteGroupBuilder group)
    {
        var faculties = group.MapGroup("/faculties");

        faculties.MapGet("/", async (
            [AsParameters] ListFaculties.Query query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListFaculties")
            .WithSummary("Fakülteleri listeler (yetki kapsamıyla sınırlı)");

        faculties.MapGet("/{id:guid}", async (
            Guid id, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetFaculty.Query(id), ct)).ToHttpResult(http))
            .WithName("GetFaculty")
            .WithSummary("Fakülte detayı");

        faculties.MapPost("/", async (
            FacultyRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateFaculty.Command(body.Code, body.Name), ct)).ToHttpResult(http))
            .WithName("CreateFaculty")
            .WithSummary("Fakülte oluşturur");

        faculties.MapPut("/{id:guid}", async (
            Guid id, FacultyRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new UpdateFaculty.Command(id, body.Code, body.Name), ct)).ToHttpResult(http))
            .WithName("UpdateFaculty")
            .WithSummary("Fakülte bilgilerini günceller");

        faculties.MapPut("/{id:guid}/status", async (
            Guid id, StatusRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new SetFacultyStatus.Command(id, body.IsActive), ct)).ToHttpResult(http))
            .WithName("SetFacultyStatus")
            .WithSummary("Fakülteyi pasifleştirir veya yeniden etkinleştirir");
    }

    private static void MapDepartments(RouteGroupBuilder group)
    {
        var departments = group.MapGroup("/departments");

        departments.MapGet("/", async (
            [AsParameters] ListDepartments.Query query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListDepartments")
            .WithSummary("Bölümleri listeler (yetki kapsamıyla sınırlı)");

        departments.MapGet("/{id:guid}", async (
            Guid id, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetDepartment.Query(id), ct)).ToHttpResult(http))
            .WithName("GetDepartment")
            .WithSummary("Bölüm detayı");

        group.MapPost("/faculties/{facultyId:guid}/departments", async (
            Guid facultyId, CreateDepartmentRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateDepartment.Command(facultyId, body.Code, body.Name), ct)).ToHttpResult(http))
            .WithName("CreateDepartment")
            .WithSummary("Fakülteye bağlı bölüm oluşturur");

        departments.MapPut("/{id:guid}", async (
            Guid id, DepartmentRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new UpdateDepartment.Command(id, body.Code, body.Name), ct)).ToHttpResult(http))
            .WithName("UpdateDepartment")
            .WithSummary("Bölüm bilgilerini günceller");

        departments.MapPut("/{id:guid}/status", async (
            Guid id, StatusRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new SetDepartmentStatus.Command(id, body.IsActive), ct)).ToHttpResult(http))
            .WithName("SetDepartmentStatus")
            .WithSummary("Bölümü pasifleştirir veya yeniden etkinleştirir");
    }

    private static void MapAcademicYears(RouteGroupBuilder group)
    {
        var years = group.MapGroup("/academic-years");

        years.MapGet("/", async (
            [AsParameters] ListAcademicYears.Query query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListAcademicYears")
            .WithSummary("Akademik yılları listeler");

        years.MapGet("/{id:guid}", async (
            Guid id, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetAcademicYear.Query(id), ct)).ToHttpResult(http))
            .WithName("GetAcademicYear")
            .WithSummary("Akademik yıl detayı");

        years.MapPost("/", async (
            AcademicYearRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateAcademicYear.Command(body.Code, body.Name, body.StartDate, body.EndDate), ct)).ToHttpResult(http))
            .WithName("CreateAcademicYear")
            .WithSummary("Akademik yıl oluşturur (academic-term.manage)");

        years.MapPost("/{yearId:guid}/terms", async (
            Guid yearId, AddTermRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new AddAcademicTerm.Command(yearId, body.TermType, body.Name, body.StartDate, body.EndDate, body.IsRegistrationOpen), ct)).ToHttpResult(http))
            .WithName("AddAcademicTerm")
            .WithSummary("Akademik yıla dönem ekler (academic-term.manage)");

        group.MapPut("/academic-terms/{termId:guid}/registration", async (
            Guid termId, SetTermRegistrationRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new SetTermRegistrationStatus.Command(termId, body.IsRegistrationOpen), ct)).ToHttpResult(http))
            .WithName("SetTermRegistrationStatus")
            .WithSummary("Dönem ders kayıt durumunu açar/kapatır (academic-term.manage)");
    }

    private static void MapPrograms(RouteGroupBuilder group)
    {
        var programs = group.MapGroup("/programs");

        programs.MapGet("/", async (
            [AsParameters] ListPrograms.Query query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListPrograms")
            .WithSummary("Akademik programları listeler (yetki kapsamıyla sınırlı)");

        programs.MapGet("/{id:guid}", async (
            Guid id, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetProgram.Query(id), ct)).ToHttpResult(http))
            .WithName("GetProgram")
            .WithSummary("Program detayı");

        group.MapPost("/departments/{departmentId:guid}/programs", async (
            Guid departmentId, ProgramRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateProgram.Command(departmentId, body.Code, body.Name, body.DegreeLevel), ct)).ToHttpResult(http))
            .WithName("CreateProgram")
            .WithSummary("Bölüme bağlı program oluşturur");

        programs.MapPut("/{id:guid}", async (
            Guid id, ProgramRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new UpdateProgram.Command(id, body.Code, body.Name, body.DegreeLevel), ct)).ToHttpResult(http))
            .WithName("UpdateProgram")
            .WithSummary("Program bilgilerini günceller");
    }

    private static void MapCurriculums(RouteGroupBuilder group)
    {
        var curriculums = group.MapGroup("/curriculums");

        curriculums.MapGet("/", async (
            [AsParameters] ListCurriculums.Query query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListCurriculums")
            .WithSummary("Müfredatları listeler (yetki kapsamıyla sınırlı)");

        curriculums.MapGet("/{id:guid}", async (
            Guid id, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetCurriculum.Query(id), ct)).ToHttpResult(http))
            .WithName("GetCurriculum")
            .WithSummary("Müfredat ve sürümlerini görüntüler");

        curriculums.MapPost("/", async (
            CreateCurriculumRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateCurriculum.Command(body.ProgramId, body.AcademicYearId, body.Code, body.Name), ct)).ToHttpResult(http))
            .WithName("CreateCurriculum")
            .WithSummary("Müfredat oluşturur (curriculum.create)");

        curriculums.MapPost("/{curriculumId:guid}/versions", async (
            Guid curriculumId, CreateCurriculumVersionRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateCurriculumVersion.Command(curriculumId, body.Notes), ct)).ToHttpResult(http))
            .WithName("CreateCurriculumVersion")
            .WithSummary("Yeni taslak müfredat sürümü oluşturur (curriculum.update)");

        curriculums.MapPost("/{curriculumId:guid}/versions/{versionId:guid}/courses", async (
            Guid curriculumId, Guid versionId, AddCurriculumCourseRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new AddCurriculumCourse.Command(
                curriculumId,
                versionId,
                body.CourseCode,
                body.CourseName,
                body.Ects,
                body.Credit,
                body.Semester,
                body.IsElective,
                body.Category,
                body.Prerequisites), ct)).ToHttpResult(http))
            .WithName("AddCurriculumCourse")
            .WithSummary("Taslak müfredat sürümüne ders ekler (curriculum.update)");

        curriculums.MapPut("/{curriculumId:guid}/versions/{versionId:guid}/publish", async (
            Guid curriculumId, Guid versionId, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new PublishCurriculumVersion.Command(curriculumId, versionId), ct)).ToHttpResult(http))
            .WithName("PublishCurriculumVersion")
            .WithSummary("Müfredat sürümünü yayınlar (curriculum.publish)");
    }
}
