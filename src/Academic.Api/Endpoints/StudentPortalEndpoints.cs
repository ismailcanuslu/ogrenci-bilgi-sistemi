using Academic.Application.Features.StudentPortal;
using Academic.Domain.Courses;
using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Academic.Api.Endpoints;

public static class StudentPortalEndpoints
{
    public static IEndpointRouteBuilder MapStudentPortalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("student-portal", 1, 0)
            .WithTags("StudentPortal (Öğrenci Paneli & Diğer Menüsü)")
            .RequireAuthorization();

        // 1. Dashboard Genel Bakış
        group.MapGet("/dashboard", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentDashboardOverviewQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentDashboardOverview")
            .WithSummary("Öğrenci ana panel özetini ve üst bar bilgilerini getirir");

        // 2. Harç Bilgileri ("Diğer" Menüsü)
        group.MapGet("/tuition", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentTuitionQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentTuition")
            .WithSummary("Öğrencinin dönemlik harç ve borç bilgilerini listeler");

        // 3. Danışman Bilgisi ("Diğer" Menüsü)
        group.MapGet("/advisor", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentAdvisorQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentAdvisor")
            .WithSummary("Öğrencinin akademik danışman bilgilerini getirir");

        // 4. Tarihçe Bilgileri ("Diğer" Menüsü)
        group.MapGet("/history", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentHistoryQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentHistory")
            .WithSummary("Öğrencinin kayıt ve akademik durum tarihçesini listeler");

        // 5. Askerlik Durumu ("Diğer" Menüsü)
        group.MapGet("/military", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentMilitaryStatusQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentMilitaryStatus")
            .WithSummary("Öğrencinin askerlik tecil ve ASAL bildirim durumunu getirir");

        // 6. IBAN Bilgileri ("Diğer" Menüsü)
        group.MapGet("/bank", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentBankDetailsQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentBankDetails")
            .WithSummary("Öğrencinin banka IBAN bilgilerini getirir");

        group.MapPut("/bank", async (UpdateStudentBankDetailsCommand body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(body, ct)).ToHttpResult(http))
            .WithName("UpdateStudentBankDetails")
            .WithSummary("Öğrencinin banka IBAN bilgilerini günceller");

        // 7. Ceza Bilgileri ("Diğer" Menüsü)
        group.MapGet("/disciplinary", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentDisciplinaryRecordsQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentDisciplinaryRecords")
            .WithSummary("Öğrencinin disiplin ve ceza kayıtlarını listeler");

        // 8. Ek Süre Bilgileri ("Diğer" Menüsü)
        group.MapGet("/study-extension", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentStudyExtensionQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentStudyExtension")
            .WithSummary("Öğrencinin azami öğrenim süresi ve ek süre/ek sınav haklarını getirir");

        // 9. Profil Resmi Yönetimi ("Diğer" Menüsü)
        group.MapGet("/profile-picture", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentProfilePictureQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentProfilePicture")
            .WithSummary("Öğrencinin profil resmini ve onay durumunu getirir");

        group.MapPost("/profile-picture", async (UploadStudentProfilePictureCommand body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(body, ct)).ToHttpResult(http))
            .WithName("UploadStudentProfilePicture")
            .WithSummary("Öğrenci profil resmi yükleme talebi oluşturur");

        // 10. Mezun İletişim Sistemi Girişi (Dashboard Kartı)
        group.MapGet("/alumni", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentAlumniInfoQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentAlumniInfo")
            .WithSummary("Mezun iletişim ağı profil bilgilerini getirir");

        group.MapPut("/alumni", async (UpdateStudentAlumniInfoCommand body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(body, ct)).ToHttpResult(http))
            .WithName("UpdateStudentAlumniInfo")
            .WithSummary("Mezun iletişim ağı profil bilgilerini günceller");

        // 11. Akademik Takvim ("Diğer" Menüsü & Dashboard Takvim Kartı)
        group.MapGet("/calendar", async (string? academicYear, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetAcademicCalendarQuery(academicYear), ct)).ToHttpResult(http))
            .WithName("GetStudentAcademicCalendar")
            .WithSummary("Üniversite akademik takvim tarihlerini listeler");

        // 12. Canlı Derslerim & Online Sınav (Dashboard Kartları)
        group.MapGet("/live-sessions", async (LiveSessionType? type, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentLiveSessionsQuery(type), ct)).ToHttpResult(http))
            .WithName("GetStudentLiveSessions")
            .WithSummary("Öğrencinin canlı ders veya online sınav oturumlarını listeler");

        // 13. Haftalık Ders Programı (Dashboard Kartı)
        group.MapGet("/weekly-schedule", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentWeeklyScheduleQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentWeeklySchedule")
            .WithSummary("Öğrencinin haftalık ders programını gün, saat, derslik ve fakülte bilgileriyle getirir");

        // 14. Transkript (Üst Bar Transkript Butonu)
        group.MapGet("/transcript", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetStudentTranscriptQuery(), ct)).ToHttpResult(http))
            .WithName("GetStudentTranscript")
            .WithSummary("Öğrencinin tüm dönem notlarını, AKTS, dönem GPA ve kümülatif CGPA transkriptini getirir");

        // 15. Kayıtlanma Onay Raporu ("Diğer" Menüsü)
        group.MapGet("/registration-report", async (string? term, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetRegistrationApprovalReportQuery(term), ct)).ToHttpResult(http))
            .WithName("GetRegistrationApprovalReport")
            .WithSummary("Dönemlik ders kayıt ve danışman onay raporunu getirir");

        return app;
    }
}
