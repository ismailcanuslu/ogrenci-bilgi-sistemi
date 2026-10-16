using InstitutionalEvaluation.Application.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InstitutionalEvaluation.Api;

public static class InstitutionalEvaluationEndpoints
{
    public static IEndpointRouteBuilder MapInstitutionalEvaluationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/institutional-evaluation")
            .WithTags("Kurumsal Değerlendirme & İstatistikler");

        // 1. Ana Sayfa Özet Paneli
        group.MapGet("/summary", async (
            IInstitutionalEvaluationRepository repo,
            CancellationToken ct) =>
        {
            var data = await repo.GetSummaryAsync(ct);
            return Results.Ok(data);
        }).AllowAnonymous(); // Öğrenci, personel ve dış paydaşlar inceleyebilir

        // 2. Öğrenci İstatistikleri (Toplam, Program, Birim, Ön Lisans, Lisans, Lisansüstü, Mezun, Uluslararası, Hazırlık, İl/Bölge, Sınıf)
        group.MapGet("/student-statistics", async (
            IInstitutionalEvaluationRepository repo,
            CancellationToken ct) =>
        {
            var data = await repo.GetStudentStatisticsAsync(ct);
            return Results.Ok(data);
        }).AllowAnonymous();

        // 3. Mezun Öğrenci İstatistikleri (Mezun sayıları, birimlere göre mezunlar)
        group.MapGet("/alumni-statistics", async (
            IInstitutionalEvaluationRepository repo,
            CancellationToken ct) =>
        {
            var data = await repo.GetAlumniStatisticsAsync(ct);
            return Results.Ok(data);
        }).AllowAnonymous();

        // 4. Personel İstatistikleri (Akademik & İdari, Görev yerine ve Kadro yerine göre)
        group.MapGet("/staff-statistics", async (
            IInstitutionalEvaluationRepository repo,
            CancellationToken ct) =>
        {
            var data = await repo.GetStaffStatisticsAsync(ct);
            return Results.Ok(data);
        }).AllowAnonymous();

        // 5. Yayınlar (Akademik Yayınlar, SCI-E, Scopus, Ulusal Hakemli)
        group.MapGet("/publications", async (
            IInstitutionalEvaluationRepository repo,
            CancellationToken ct) =>
        {
            var data = await repo.GetPublicationStatisticsAsync(ct);
            return Results.Ok(data);
        }).AllowAnonymous();

        // 6. Birim İstatistikleri (Türlerine göre birim, akademik program sayıları)
        group.MapGet("/unit-statistics", async (
            IInstitutionalEvaluationRepository repo,
            CancellationToken ct) =>
        {
            var data = await repo.GetUnitStatisticsAsync(ct);
            return Results.Ok(data);
        }).AllowAnonymous();

        // 7. Göstergeler (Öğrenci, Personel, Fiziki Alanlar, Fiziki Alan Oranları, Bologna Göstergeleri)
        group.MapGet("/indicators", async (
            IInstitutionalEvaluationRepository repo,
            CancellationToken ct) =>
        {
            var data = await repo.GetIndicatorsAsync(ct);
            return Results.Ok(data);
        }).AllowAnonymous();

        // 8. BAP İstatistikleri (Proje sayıları, bütçeler, türler)
        group.MapGet("/bap-statistics", async (
            IInstitutionalEvaluationRepository repo,
            CancellationToken ct) =>
        {
            var data = await repo.GetBapStatisticsAsync(ct);
            return Results.Ok(data);
        }).AllowAnonymous();

        // 9. Uzaktan Eğitim İstatistikleri (Senkron-Asenkron, sınav, ders ve katılım oranları)
        group.MapGet("/distance-education-statistics", async (
            IInstitutionalEvaluationRepository repo,
            CancellationToken ct) =>
        {
            var data = await repo.GetDistanceEducationStatisticsAsync(ct);
            return Results.Ok(data);
        }).AllowAnonymous();

        return app;
    }
}
