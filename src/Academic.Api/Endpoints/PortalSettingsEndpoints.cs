using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Academic.Api.Endpoints;

public static class PortalSettingsEndpoints
{
    private static PortalSettingsDto _settings = new()
    {
        BackgroundMode = "slideshow", // "slideshow" | "single" | "empty"
        SlideIntervalSeconds = 5,
        CoverImages = ["/cover-1.jpg", "/cover-2.jpg"],
        AboutTitle = "T.C. Anadolu Devlet Üniversitesi - Öğrenci Bilgi Sistemi (OBS)",
        AboutContent = "Üniversitemiz Öğrenci Bilgi Sistemi (OBS), çağdaş yükseköğretim standartlarına uygun, güvenli, hızlı ve kullanıcı odaklı bir dijital kampüs deneyimi sunmaktadır. Sistemimiz YÖKAK akreditasyon süreçleri, Bologna uyumu, dijital transkript doğrulama ve engelsiz erişim standartlarıyla tam entegredir.",
        Announcements =
        [
            new PortalAnnouncementDto(
                1,
                "2026-2027 Güz Yarıyılı Ders Kayıt ve Danışman Onayları",
                "15 Ekim 2026",
                "Ders kayıt işlemleri 15-20 Ekim 2026 tarihleri arasında portal üzerinden gerçekleştirilecektir. Danışman onaylarının son günü 21 Ekim 2026 saat 17:00'dir.",
                "High",
                "Akademik Takvim"),
            new PortalAnnouncementDto(
                2,
                "Çift Anadal (ÇAP) ve Yandal Başvuruları Başladı",
                "12 Ekim 2026",
                "Genel Not Ortalaması (GNO) 3.00 ve üzeri olan öğrencilerimiz Öğrenci Başvuruları sekmesinden çevrimiçi başvuru yapabilirler.",
                "Medium",
                "Başvuru"),
            new PortalAnnouncementDto(
                3,
                "obsKimlik Güvenli Dijital Kampüs Kartı Geçişi",
                "10 Ekim 2026",
                "Tüm öğrencilerimiz ve akademik personelimiz yemekhane, kütüphane ve laboratuvar geçişlerinde yeni obsKimlik dijital karekodlarını kullanabilir.",
                "Normal",
                "Genel")
        ]
    };

    public static IEndpointRouteBuilder MapPortalSettingsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/portal/settings")
            .WithTags("PortalSettings");

        group.MapGet("/", () => Results.Ok(_settings))
            .AllowAnonymous()
            .WithName("GetPortalSettings")
            .WithSummary("Giriş ekranı ve genel portal ayarlarını döner");

        group.MapPut("/", (PortalSettingsDto updateRequest) =>
        {
            if (updateRequest is null)
            {
                return Results.BadRequest(new { error = "Geçersiz ayar içeriği." });
            }

            _settings = updateRequest;
            return Results.Ok(new { success = true, data = _settings });
        })
        .AllowAnonymous()
        .WithName("UpdatePortalSettings")
        .WithSummary("Öğrenci işleri tarafından giriş ekranı ve portal ayarlarını günceller");

        return app;
    }
}

public sealed record PortalSettingsDto
{
    public string BackgroundMode { get; set; } = "slideshow";
    public int SlideIntervalSeconds { get; set; } = 5;
    public List<string> CoverImages { get; set; } = ["/cover-1.jpg", "/cover-2.jpg"];
    public string AboutTitle { get; set; } = "";
    public string AboutContent { get; set; } = "";
    public List<PortalAnnouncementDto> Announcements { get; set; } = [];
}

public sealed record PortalAnnouncementDto(
    int Id,
    string Title,
    string Date,
    string Content,
    string Priority,
    string Category);
