using Academic.Application.Features.Dashboard;
using Academic.Domain.Dashboard;
using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Academic.Api.Endpoints;

public sealed record AddWidgetRequest(
    WidgetType Type,
    string? CustomTitle,
    int ColumnSpan = 1,
    string? ConfigurationJson = null);

public sealed record UpdateWidgetRequest(
    string? CustomTitle,
    int Order,
    int ColumnSpan,
    string? ConfigurationJson,
    bool IsVisible);

public sealed record AddShortcutRequest(
    string Title,
    string TargetUrl,
    string? Icon,
    string? Color);

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("dashboard", 1, 0)
            .WithTags("Dashboard (Widget ve Kısayol Yönetimi)")
            .RequireAuthorization();

        // 1. Kullanılabilir Widget Kataloğu
        group.MapGet("/available-widgets", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetAvailableWidgetsQuery(), ct)).ToHttpResult(http))
            .WithName("GetAvailableWidgets")
            .WithSummary("Sistemde kullanıcının ekleyebileceği tüm widget kataloğunu listeler");

        // 2. Kullanıcının Dashboard Widget'ları
        group.MapGet("/widgets", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetUserWidgetsQuery(), ct)).ToHttpResult(http))
            .WithName("GetUserWidgets")
            .WithSummary("Kullanıcının dashboardundaki aktif widget'ları getirir");

        // 3. Yeni Widget Ekleme ("+ Yeni Widget Ekle" butonu)
        group.MapPost("/widgets", async (AddWidgetRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new AddUserWidgetCommand(body.Type, body.CustomTitle, body.ColumnSpan, body.ConfigurationJson), ct)).ToHttpResult(http))
            .WithName("AddUserWidget")
            .WithSummary("Kullanıcının ana ekranına yeni bir widget ekler");

        // 4. Widget Güncelleme (Sıralama ve Kolon Boyutu)
        group.MapPut("/widgets/{widgetId:guid}", async (Guid widgetId, UpdateWidgetRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new UpdateUserWidgetCommand(widgetId, body.CustomTitle, body.Order, body.ColumnSpan, body.ConfigurationJson, body.IsVisible), ct)).ToHttpResult(http))
            .WithName("UpdateUserWidget")
            .WithSummary("Widget başlığını, sırasını veya genişliğini günceller");

        // 5. Widget Kaldırma (Kırmızı Kapatma Çarpısı)
        group.MapDelete("/widgets/{widgetId:guid}", async (Guid widgetId, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new RemoveUserWidgetCommand(widgetId), ct)).ToHttpResult(http))
            .WithName("RemoveUserWidget")
            .WithSummary("Widget'ı kullanıcının dashboardundan kaldırır");

        // 6. Kişisel Kısayollar
        group.MapGet("/shortcuts", async (ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetUserShortcutsQuery(), ct)).ToHttpResult(http))
            .WithName("GetUserShortcuts")
            .WithSummary("Kullanıcının kişisel kısayollarını listeler");

        group.MapPost("/shortcuts", async (AddShortcutRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new AddUserShortcutCommand(body.Title, body.TargetUrl, body.Icon, body.Color), ct)).ToHttpResult(http))
            .WithName("AddUserShortcut")
            .WithSummary("Kişisel kısayol ekler");

        group.MapDelete("/shortcuts/{shortcutId:guid}", async (Guid shortcutId, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new RemoveUserShortcutCommand(shortcutId), ct)).ToHttpResult(http))
            .WithName("RemoveUserShortcut")
            .WithSummary("Kişisel kısayolu siler");

        return app;
    }
}
