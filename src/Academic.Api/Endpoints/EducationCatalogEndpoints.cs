using Academic.Application.Abstractions;
using Academic.Domain.Courses;
using Academic.Domain.Programs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Academic.Api.Endpoints;

public static class EducationCatalogEndpoints
{
    public static IEndpointRouteBuilder MapEducationCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/education-catalog")
            .WithTags("Eğitim Kataloğu / Müfredat Portalı");

        // 1. Sol Menü: Akademik Hiyerarşi Ağacı (Eğitim Düzeyine göre filtrelenebilir: Lisans, Ön Lisans, YL, Doktora)
        group.MapGet("/tree", async (
            DegreeLevel? degreeLevel,
            IEducationCatalogRepository repo,
            CancellationToken ct) =>
        {
            var tree = await repo.GetAcademicHierarchyTreeAsync(degreeLevel, ct);
            return Results.Ok(tree);
        }).AllowAnonymous(); // Müfredatlar ve katalog herkese açık

        // 2. Program Tanımı (Tarihçe, Kabul Koşulları, İstihdam Profilleri, Mezuniyet Şartları vb.)
        group.MapGet("/programs/{programId:guid}/profile", async (
            Guid programId,
            IEducationCatalogRepository repo,
            CancellationToken ct) =>
        {
            var profile = await repo.GetProgramProfileAsync(programId, ct);
            return Results.Ok(profile);
        }).AllowAnonymous();

        // 3. Program Çıktıları
        group.MapGet("/programs/{programId:guid}/outcomes", async (
            Guid programId,
            IEducationCatalogRepository repo,
            CancellationToken ct) =>
        {
            var outcomes = await repo.GetProgramOutcomesAsync(programId, ct);
            return Results.Ok(outcomes);
        }).AllowAnonymous();

        // 4. Programın Öğretim Planı (Dönem bazlı 1. Yarıyıl, 2. Yarıyıl dersleri, T, U, L, Kredi, AKTS, Zorunlu/Seçmeli)
        group.MapGet("/programs/{programId:guid}/teaching-plan", async (
            Guid programId,
            Guid? versionId,
            IEducationCatalogRepository repo,
            CancellationToken ct) =>
        {
            var plan = await repo.GetProgramTeachingPlanAsync(programId, versionId, ct);
            return plan == null ? Results.NotFound(new { Message = "Program için aktif bir öğretim planı / müfredat bulunamadı." }) : Results.Ok(plan);
        }).AllowAnonymous();

        // 5. Ders Detayı ve Öğrenme Çıktıları (Dersin Dili, Tipi, Ön Koşul, Amacı, İçeriği, Kaynaklar, Öğretim Elemanı, Çıktılar)
        group.MapGet("/courses/{courseCode}/syllabus", async (
            string courseCode,
            IEducationCatalogRepository repo,
            CancellationToken ct) =>
        {
            var syllabus = await repo.GetCourseSyllabusAsync(courseCode, ct);
            return syllabus == null ? Results.NotFound(new { Message = "Ders izlencesi bulunamadı." }) : Results.Ok(syllabus);
        }).AllowAnonymous();

        // 6. Yetkili / Bölüm Başkanı Program Tanımını Güncelleme
        group.MapPut("/programs/{programId:guid}/profile", async (
            Guid programId,
            UpdateProgramProfileDto dto,
            IEducationCatalogRepository repo,
            CancellationToken ct) =>
        {
            await repo.UpdateProgramProfileAsync(programId, dto, ct);
            return Results.Ok();
        }).RequireAuthorization();

        // 7. Yetkili / Ders Sorumlusu Ders İzlencesini Kaydetme
        group.MapPut("/courses/{courseCode}/syllabus", async (
            string courseCode,
            SaveSyllabusRequest req,
            IEducationCatalogRepository repo,
            CancellationToken ct) =>
        {
            var syllabus = CourseSyllabus.Create(
                courseCode,
                req.Language,
                req.Level,
                req.Type,
                req.PrerequisitesText,
                req.Aim,
                req.Content,
                req.TextbooksAndResources,
                req.PlannedLearningActivities,
                req.InstructorsText,
                req.LearningOutcomesJson,
                req.TheoryHours,
                req.PracticeHours,
                req.LabHours,
                req.LocalCredits,
                req.Ects);

            await repo.SaveCourseSyllabusAsync(syllabus, ct);
            return Results.Ok();
        }).RequireAuthorization();

        return app;
    }
}

public record SaveSyllabusRequest(
    string Language,
    string Level,
    string Type,
    string PrerequisitesText,
    string Aim,
    string Content,
    string TextbooksAndResources,
    string PlannedLearningActivities,
    string InstructorsText,
    string LearningOutcomesJson,
    int TheoryHours,
    int PracticeHours,
    int LabHours,
    int LocalCredits,
    int Ects);
