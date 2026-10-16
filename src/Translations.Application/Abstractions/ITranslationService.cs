namespace Translations.Application.Abstractions;

public interface ITranslationService
{
    string Translate(string code, string? culture = null);

    IReadOnlyDictionary<string, string> GetAll(string? culture = null);

    string CurrentCulture { get; }
}
