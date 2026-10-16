using Microsoft.Extensions.Options;
using Notifications.Domain.Notifications;
using Notifications.Infrastructure.Options;
using Notifications.Infrastructure.Templates;
using Xunit;

namespace UniversalSystem.UnitTests.Notifications;

public class SimpleTemplateRendererTests
{
    private static SimpleTemplateRenderer CreateRenderer()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new NotificationOptions
        {
            Branding = new NotificationBrandingOptions
            {
                CompanyName = "OBS Platform",
                LogoUrl = "https://example.com/logo.png",
                PrimaryColor = "#6166BE"
            }
        });
        return new SimpleTemplateRenderer(options);
    }

    [Fact]
    public async Task RenderAsync_WithOtpCode_ShouldRenderOtpBox()
    {
        // Arrange
        var renderer = CreateRenderer();
        var parameters = new Dictionary<string, string>
        {
            ["Heading"] = "Doğrulama Kodunuz:",
            ["Message"] = "Giriş yapmak için bu kodu kullanın.",
            ["OtpCode"] = "050970",
            ["ExpiresInMinutes"] = "5"
        };

        // Act
        var result = await renderer.RenderAsync(NotificationTemplates.Security.OtpVerification, parameters);

        // Assert
        Assert.Contains("050970", result);
        Assert.Contains("Doğrulama Kodunuz:", result);
        Assert.Contains("Bu kod 5 dakika süreyle geçerlidir.", result);
        Assert.Contains("https://example.com/logo.png", result);
        Assert.DoesNotContain("Devam Et", result); // Action butonu olmamalı, sadece OTP kutusu olmalı
    }

    [Fact]
    public async Task RenderAsync_WithButtonUrl_ShouldRenderActionButton()
    {
        // Arrange
        var renderer = CreateRenderer();
        var parameters = new Dictionary<string, string>
        {
            ["Heading"] = "Hesabınızı Doğrulayın",
            ["Message"] = "Aşağıdaki butona tıklayarak devam edin.",
            ["ButtonText"] = "Hesabımı Doğrula",
            ["ButtonUrl"] = "https://obs.uni.edu.tr/verify?token=123",
            ["ExpiresInMinutes"] = "15"
        };

        // Act
        var result = await renderer.RenderAsync(NotificationTemplates.Security.EmailVerification, parameters);

        // Assert
        Assert.Contains("Hesabımı Doğrula", result);
        Assert.Contains("https://obs.uni.edu.tr/verify?token=123", result);
        Assert.Contains("Bu kod 15 dakika süreyle geçerlidir.", result);
    }

    [Fact]
    public async Task RenderAsync_WithCleanTheme_ShouldRenderStripeCleanLayout()
    {
        // Arrange
        var renderer = CreateRenderer();
        var parameters = new Dictionary<string, string>
        {
            ["Theme"] = "Clean",
            ["Heading"] = "Your Link verification code:",
            ["Message"] = "This code will expire in 10 minutes.",
            ["OtpCode"] = "606428",
            ["Preheader"] = "Verify your email address to sign in."
        };

        // Act
        var result = await renderer.RenderAsync(NotificationTemplates.Common.Transactional, parameters);

        // Assert
        Assert.Contains("606428", result);
        Assert.Contains("Your Link verification code:", result);
        Assert.Contains("Verify your email address to sign in.", result);
        Assert.Contains("table bgcolor=\"#e6ffed\"", result); // Stripe tarzı yeşil kutu
    }

    [Fact]
    public async Task RenderAsync_WithCustomTemplate_ShouldSubstituteTokens()
    {
        // Arrange
        var renderer = CreateRenderer();
        var customTemplate = "Sayın {{Student}}, {{Course}} dersi notunuz: {{Grade}}";
        var parameters = new Dictionary<string, string>
        {
            ["Student"] = "Zeynep Kaya",
            ["Course"] = "Yazılım Mimarisi",
            ["Grade"] = "AA"
        };

        // Act
        var result = await renderer.RenderAsync(customTemplate, parameters);

        // Assert
        Assert.Equal("Sayın Zeynep Kaya, Yazılım Mimarisi dersi notunuz: AA", result);
    }
}
