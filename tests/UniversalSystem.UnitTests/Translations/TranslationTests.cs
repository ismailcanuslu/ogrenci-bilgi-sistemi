using Microsoft.AspNetCore.Http;
using NSubstitute;
using Translations.Application.Abstractions;
using Translations.Domain;
using Translations.Infrastructure.Services;
using Xunit;

namespace UniversalSystem.UnitTests.Translations;

public class TranslationTests
{
    [Fact]
    public void Translate_ShouldReturnCorrectText_ForSupportedLanguages()
    {
        // Act
        var tr = TranslationCatalog.Translate("Security.InvalidCredentials", "tr");
        var en = TranslationCatalog.Translate("Security.InvalidCredentials", "en");
        var de = TranslationCatalog.Translate("Security.InvalidCredentials", "de");
        var ru = TranslationCatalog.Translate("Security.InvalidCredentials", "ru");

        // Assert
        Assert.Equal("Geçersiz e-posta veya parola.", tr);
        Assert.Equal("Invalid email or password.", en);
        Assert.Equal("Ungültige E-Mail-Adresse oder Passwort.", de);
        Assert.Equal("Неверный адрес электронной почты или пароль.", ru);
    }

    [Fact]
    public void Translate_ShouldFallbackToTurkish_WhenCultureIsUnsupported()
    {
        // Act (French is unsupported, should fallback to Turkish)
        var result = TranslationCatalog.Translate("Security.InvalidCredentials", "fr");

        // Assert
        Assert.Equal("Geçersiz e-posta veya parola.", result);
    }

    [Fact]
    public void Translate_ShouldFallbackToTurkish_WhenKeyMissingInTargetLanguage()
    {
        // If a code exists in TR but not in target, it must fallback to TR
        var result = TranslationCatalog.Translate("Security.UserNotFound", "de");
        Assert.Equal("Benutzer nicht gefunden.", result);
    }

    [Fact]
    public void Translate_ShouldReturnCodeItself_WhenKeyDoesNotExistAtAll()
    {
        // Act
        var result = TranslationCatalog.Translate("NonExistent.Key.123", "en");

        // Assert
        Assert.Equal("NonExistent.Key.123", result);
    }

    [Fact]
    public void LanguageCode_Normalize_ShouldNormalizeCorrectly()
    {
        Assert.Equal("tr", LanguageCode.Normalize(null));
        Assert.Equal("tr", LanguageCode.Normalize(string.Empty));
        Assert.Equal("tr", LanguageCode.Normalize("tr-TR"));
        Assert.Equal("en", LanguageCode.Normalize("en-US"));
        Assert.Equal("de", LanguageCode.Normalize("de-DE"));
        Assert.Equal("ru", LanguageCode.Normalize("ru-RU"));
        Assert.Equal("tr", LanguageCode.Normalize("ja-JP")); // unknown culture -> default to TR
    }

    [Fact]
    public void TranslationService_ShouldTranslateUsingService()
    {
        var accessor = Substitute.For<IHttpContextAccessor>();
        ITranslationService service = new TranslationService(accessor);

        var translated = service.Translate("Files.FileNotFound", "en");
        Assert.Equal("Requested file was not found.", translated);

        var fallback = service.Translate("Files.FileNotFound", "invalid-lang");
        Assert.Equal("İstenen dosya bulunamadı.", fallback);
    }
}
