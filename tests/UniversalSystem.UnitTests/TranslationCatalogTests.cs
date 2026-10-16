using Translations.Domain;
using Xunit;

namespace UniversalSystem.UnitTests;

public class TranslationCatalogTests
{
    [Theory]
    [InlineData("tr")]
    [InlineData("en")]
    [InlineData("de")]
    [InlineData("ru")]
    [InlineData("ge")]
    public void IsSupported_SupportedLanguages_ReturnsTrue(string lang)
    {
        Assert.True(LanguageCode.IsSupported(lang));
    }

    [Fact]
    public void Translate_ObsKeys_ShouldReturnTranslationsInAllFiveLanguages()
    {
        var testKeys = new[]
        {
            "Obs.SelectAcademicProgram",
            "Obs.Transcript",
            "Obs.CourseListWidget",
            "Obs.TuitionInfo",
            "Obs.ElectronicDocumentManagement",
            "Obs.SocialContribution"
        };

        var languages = new[] { "tr", "en", "de", "ru", "ge" };

        foreach (var lang in languages)
        {
            foreach (var key in testKeys)
            {
                var translated = TranslationCatalog.Translate(key, lang);
                Assert.False(string.IsNullOrWhiteSpace(translated), $"Translation missing for {key} in {lang}");
                Assert.NotEqual(key, translated); // Not falling back to key
            }
        }

        // Test specific Georgian translation
        Assert.Equal("ტრანსკრიპტი", TranslationCatalog.Translate("Obs.Transcript", "ge"));
        Assert.Equal("სოციალური პასუხისმგებლობის კოორდინაცია", TranslationCatalog.Translate("Obs.SocialContribution", "ge"));
    }
}
