using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Translations.Application.Features;
using Translations.Domain;

namespace Translations.Api.Endpoints;

public static class TranslationEndpoints
{
    public static IEndpointRouteBuilder MapTranslationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("translations", 1, 0)
            .WithTags("Translations");

        group.MapGet("/", async (
            string? lang,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTranslationsCatalog.Query(lang);
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("GetTranslationsCatalog")
        .WithSummary("Sistemdeki tüm hata ve mesaj çevirilerini listeleme (tr, en, de, ru)");

        group.MapGet("/{code}", async (
            string code,
            string? lang,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTranslation.Query(code, lang);
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("GetTranslation")
        .WithSummary("Belirli bir hata veya mesaj kodunun hedef dildeki çevirisini getirme");

        group.MapGet("/languages", () =>
        {
            return Results.Ok(new
            {
                Default = LanguageCode.Default,
                Supported = LanguageCode.SupportedLanguages
            });
        })
        .WithName("GetSupportedLanguages")
        .WithSummary("Desteklenen diller listesi (Varsayılan: Türkçe 'tr')");

        return app;
    }
}
