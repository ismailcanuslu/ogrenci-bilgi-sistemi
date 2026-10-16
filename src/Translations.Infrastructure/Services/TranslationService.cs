using Microsoft.AspNetCore.Http;
using Translations.Application.Abstractions;
using Translations.Domain;

namespace Translations.Infrastructure.Services;

public sealed class TranslationService(IHttpContextAccessor httpContextAccessor) : ITranslationService
{
    public string CurrentCulture => ResolveCurrentCulture();

    public string Translate(string code, string? culture = null)
    {
        var targetCulture = !string.IsNullOrWhiteSpace(culture)
            ? culture
            : ResolveCurrentCulture();

        return TranslationCatalog.Translate(code, targetCulture);
    }

    public IReadOnlyDictionary<string, string> GetAll(string? culture = null)
    {
        var targetCulture = !string.IsNullOrWhiteSpace(culture)
            ? culture
            : ResolveCurrentCulture();

        return TranslationCatalog.GetAll(targetCulture);
    }

    private string ResolveCurrentCulture()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return LanguageCode.Default;
        }

        // 1. Check query parameter: ?lang=en
        if (httpContext.Request.Query.TryGetValue("lang", out var queryLang) &&
            !string.IsNullOrWhiteSpace(queryLang))
        {
            return LanguageCode.Normalize(queryLang);
        }

        // 2. Check Accept-Language header: en-US,en;q=0.9,tr;q=0.8
        var acceptLanguage = httpContext.Request.Headers.AcceptLanguage.ToString();
        if (!string.IsNullOrWhiteSpace(acceptLanguage))
        {
            // First language in header
            var firstLang = acceptLanguage.Split(',')[0].Trim();
            return LanguageCode.Normalize(firstLang);
        }

        return LanguageCode.Default;
    }
}
