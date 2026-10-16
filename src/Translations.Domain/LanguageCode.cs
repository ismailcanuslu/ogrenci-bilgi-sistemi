namespace Translations.Domain;

public static class LanguageCode
{
    public const string Turkish = "tr";
    public const string English = "en";
    public const string German = "de";
    public const string Russian = "ru";
    public const string Georgian = "ge";

    public const string Default = Turkish;

    public static readonly IReadOnlySet<string> SupportedLanguages = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        Turkish,
        English,
        German,
        Russian,
        Georgian
    };

    public static bool IsSupported(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var normalized = Normalize(code);
        return SupportedLanguages.Contains(normalized);
    }

    public static string Normalize(string? culture)
    {
        if (string.IsNullOrWhiteSpace(culture))
        {
            return Default;
        }

        var trimmed = culture.Trim();
        var dashIndex = trimmed.IndexOf('-');
        var langPart = dashIndex > 0 ? trimmed[..dashIndex] : trimmed;
        langPart = langPart.ToLowerInvariant();

        return SupportedLanguages.Contains(langPart) ? langPart : Default;
    }
}
