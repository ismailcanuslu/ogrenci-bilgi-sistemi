using Microsoft.Extensions.Options;
using Notifications.Application.Abstractions.Templates;
using Notifications.Domain.Notifications;
using Notifications.Infrastructure.Options;

namespace Notifications.Infrastructure.Templates;

public sealed class SimpleTemplateRenderer(IOptions<NotificationOptions> options) : ITemplateRenderer
{
    private readonly NotificationBrandingOptions _branding = options.Value.Branding;

    public Task<string> RenderAsync(
        string templateCode,
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken = default)
    {
        var theme = parameters.TryGetValue("Theme", out var specifiedTheme) && !string.IsNullOrWhiteSpace(specifiedTheme)
            ? specifiedTheme
            : _branding.DefaultTheme;

        var template = ResolveTemplate(templateCode, theme);

        var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["CompanyName"] = _branding.CompanyName,
            ["LogoUrl"] = _branding.LogoUrl,
            ["SupportUrl"] = _branding.SupportUrl,
            ["SupportEmail"] = _branding.SupportEmail,
            ["CompanyAddress"] = _branding.CompanyAddress,
            ["PrimaryColor"] = _branding.PrimaryColor,
            ["AccentColor"] = _branding.AccentColor
        };

        foreach (var (key, value) in parameters)
        {
            merged[key] = value;
        }

        // Render dynamic action block (OTP Code box vs Button)
        merged["ActionBlock"] = RenderActionBlock(merged, theme);

        // Render dynamic expiration notice
        merged["ExpirationNotice"] = RenderExpirationNotice(merged, theme);

        // Render dynamic disclaimer
        merged["Disclaimer"] = RenderDisclaimer(merged, theme);

        // Render preheader
        merged["Preheader"] = merged.TryGetValue("Preheader", out var preheader) && !string.IsNullOrWhiteSpace(preheader)
            ? preheader
            : string.Empty;

        var rendered = template;
        foreach (var (key, value) in merged)
        {
            rendered = rendered.Replace($"{{{{{key}}}}}", value, StringComparison.OrdinalIgnoreCase);
        }

        return Task.FromResult(rendered);
    }

    private static readonly HashSet<string> MasterTemplateCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        NotificationTemplates.Common.Transactional,
        NotificationTemplates.Security.OtpVerification,
        NotificationTemplates.Security.EmailVerification,
        NotificationTemplates.Security.PasswordReset,
        NotificationTemplates.Security.Welcome,
        "ModernCard",
        "Clean"
    };

    private static string ResolveTemplate(string templateCode, string theme)
    {
        if (string.Equals(templateCode, "Clean", StringComparison.OrdinalIgnoreCase) ||
            (string.Equals(theme, "Clean", StringComparison.OrdinalIgnoreCase) && MasterTemplateCodes.Contains(templateCode)))
        {
            return StripeCleanHtmlTemplate;
        }

        if (MasterTemplateCodes.Contains(templateCode))
        {
            return RedotPayModernHtmlTemplate;
        }

        return templateCode;
    }

    private static string RenderActionBlock(IReadOnlyDictionary<string, string> p, string theme)
    {
        var hasOtp = p.TryGetValue("OtpCode", out var otp) && !string.IsNullOrWhiteSpace(otp);
        var hasButton = p.TryGetValue("ButtonUrl", out var btnUrl) && !string.IsNullOrWhiteSpace(btnUrl);
        var btnText = p.TryGetValue("ButtonText", out var text) && !string.IsNullOrWhiteSpace(text)
            ? text
            : "Devam Et";

        if (hasOtp)
        {
            return string.Equals(theme, "Clean", StringComparison.OrdinalIgnoreCase)
                ? $"""
                  <table bgcolor="#e6ffed" border="0" cellpadding="0" cellspacing="0" style="border-radius:10px;margin:20px auto;width:fit-content;">
                    <tr>
                      <td style="padding:16px 28px;color:#171717;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,sans-serif;font-weight:600;font-size:32px;line-height:40px;letter-spacing:8px;text-align:center;">
                        {otp}
                      </td>
                    </tr>
                  </table>
                  """
                : $"""
                  <table role="presentation" cellpadding="0" cellspacing="0" border="0" align="center" style="margin:24px auto;">
                    <tr>
                      <td align="center" style="font-family:Arial,sans-serif;font-size:44px;line-height:34px;font-weight:bold;letter-spacing:12px;padding:28px 24px;border-radius:8px;background-color:{p.GetValueOrDefault("AccentColor", "#F0F0F9")};color:{p.GetValueOrDefault("PrimaryColor", "#6166BE")};">
                        {otp}
                      </td>
                    </tr>
                  </table>
                  """;
        }

        if (hasButton)
        {
            var primaryColor = p.GetValueOrDefault("PrimaryColor", "#6166BE");
            return $"""
                  <table role="presentation" cellpadding="0" cellspacing="0" border="0" align="center" style="margin:24px auto;">
                    <tr>
                      <td align="center">
                        <a href="{btnUrl}" style="display:inline-block;padding:16px 36px;font-family:Arial,sans-serif;font-size:18px;font-weight:bold;color:#FFFFFF;background-color:{primaryColor};text-decoration:none;border-radius:8px;">
                          {btnText}
                        </a>
                      </td>
                    </tr>
                  </table>
                  """;
        }

        return string.Empty;
    }

    private static string RenderExpirationNotice(IReadOnlyDictionary<string, string> p, string theme)
    {
        if (p.TryGetValue("ExpiresInMinutes", out var minutes) && !string.IsNullOrWhiteSpace(minutes))
        {
            var font = string.Equals(theme, "Clean", StringComparison.OrdinalIgnoreCase)
                ? "font-family:'SF Pro Text',Arial,sans-serif;color:#1d3944;font-size:15px;line-height:22px;text-align:center;"
                : "font-family:Arial,sans-serif;color:#000000;font-size:16px;line-height:24px;text-align:center;";

            return $"""
                  <tr>
                    <td align="center" style="{font}padding:8px 0;">
                      Bu kod {minutes} dakika süreyle geçerlidir.
                    </td>
                  </tr>
                  """;
        }

        return string.Empty;
    }

    private static string RenderDisclaimer(IReadOnlyDictionary<string, string> p, string theme)
    {
        var disclaimerText = p.TryGetValue("Disclaimer", out var text) && !string.IsNullOrWhiteSpace(text)
            ? text
            : "Bu işlemi siz talep etmediyseniz, bu e-postayı güvenle yok sayabilirsiniz.";

        var font = string.Equals(theme, "Clean", StringComparison.OrdinalIgnoreCase)
            ? "font-family:'SF Pro Text',Arial,sans-serif;color:#707070;font-size:13px;line-height:20px;text-align:center;"
            : "font-family:Arial,sans-serif;color:#B2B2B2;font-size:14px;line-height:22px;text-align:center;";

        return $"""
              <tr>
                <td align="center" style="{font}padding:12px 0;">
                  {disclaimerText}
                </td>
              </tr>
              """;
    }

    private const string RedotPayModernHtmlTemplate = """
<!doctype html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <title>{{Subject}}</title>
  <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <style type="text/css">
    * { line-height: inherit; }
    body { margin: 0; padding: 0; background-color: #F5F5F7; font-family: Arial, sans-serif; }
    table, tr, td { vertical-align: top; border-collapse: collapse; }
    img { border: 0; outline: none; text-decoration: none; }
  </style>
</head>
<body style="width:100%;background-color:#F5F5F7;margin:0;padding:24px 0;">
  <div style="width:100%;background-color:#F5F5F7;">
    <div style="margin:0 auto;max-width:616px;">
      <table role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0" style="table-layout:fixed;width:100%;">
        <tbody>
          <tr>
            <td>
              <div style="width:100%;max-width:600px;margin:0 auto;background-color:#FFFFFF;border-radius:20px;box-shadow:0 4px 12px rgba(0,0,0,0.05);overflow:hidden;">
                <table role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0" style="background-color:#FFFFFF;width:100%;border-radius:20px;padding:32px 24px;">
                  <tbody>
                    <tr>
                      <td align="center" style="padding-bottom:32px;">
                        <img alt="{{CompanyName}}" height="36" src="{{LogoUrl}}" style="display:block;border:0;outline:none;text-decoration:none;max-height:36px;">
                      </td>
                    </tr>
                    <tr>
                      <td align="center" style="overflow-wrap:break-word;word-break:break-word;padding-bottom:16px;">
                        <div style="font-family:Arial,sans-serif;font-size:32px;line-height:40px;font-weight:bold;letter-spacing:-0.5px;text-align:center;color:#000000;">
                          {{Heading}}
                        </div>
                      </td>
                    </tr>
                    <tr>
                      <td align="center" style="font-family:Arial,sans-serif;color:#4A4A4A;font-size:16px;line-height:26px;text-align:center;padding-bottom:8px;">
                        {{Message}}
                      </td>
                    </tr>
                    <tr>
                      <td align="center">
                        {{ActionBlock}}
                      </td>
                    </tr>
                    {{ExpirationNotice}}
                    {{Disclaimer}}
                    <tr>
                      <td style="border-top:1px solid #EBEBF0;padding-top:24px;margin-top:24px;text-align:center;">
                        <p style="font-family:Arial,sans-serif;color:#8E8E93;font-size:12px;line-height:18px;margin:0;">
                          {{CompanyName}} &bull; {{CompanyAddress}}<br>
                          Sorularınız için <a href="{{SupportUrl}}" style="color:{{PrimaryColor}};text-decoration:none;">Destek Masası</a>
                        </p>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</body>
</html>
""";

    private const string StripeCleanHtmlTemplate = """
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
  <meta name="viewport" content="width=device-width">
  <title>{{Subject}}</title>
  <style>
    body, td, a { font-family: 'SF Pro Text', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; }
    td { color: #1d3944; }
  </style>
</head>
<body bgcolor="#f5f5f5" style="border:0;margin:0;padding:24px 0;width:100%;background-color:#f5f5f5;">
  <!-- Preheader preview text -->
  <div style="display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden;">
    {{Preheader}} &zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
  </div>
  <table align="center" bgcolor="#ffffff" border="0" cellpadding="0" cellspacing="0" width="600" style="border-radius:6px;margin:0 auto;width:600px;background-color:#ffffff;box-shadow:0 1px 3px rgba(0,0,0,0.08);">
    <tbody>
      <tr>
        <td style="padding:24px 36px 16px 36px;">
          <img alt="{{CompanyName}}" height="28" style="display:block;border:0;max-height:28px;" src="{{LogoUrl}}">
        </td>
      </tr>
      <tr>
        <td style="border-top:1px solid #e6ebf1;padding:0 36px;"></td>
      </tr>
      <tr>
        <td style="padding:28px 36px 12px 36px;font-size:16px;line-height:24px;color:#1d3944;">
          <h2 style="margin:0 0 12px 0;font-size:22px;line-height:30px;color:#1d3944;">{{Heading}}</h2>
          <p style="margin:0;color:#1d3944;font-size:16px;line-height:24px;">{{Message}}</p>
        </td>
      </tr>
      <tr>
        <td align="center" style="padding:0 36px;">
          {{ActionBlock}}
        </td>
      </tr>
      <tr>
        <td style="padding:0 36px;">
          <table width="100%" border="0" cellpadding="0" cellspacing="0">
            {{ExpirationNotice}}
            {{Disclaimer}}
          </table>
        </td>
      </tr>
      <tr>
        <td style="border-top:1px solid #e6ebf1;margin-top:24px;padding:24px 36px;color:#707070;font-size:12px;line-height:18px;text-align:center;">
          {{CompanyName}} &bull; {{CompanyAddress}}<br>
          <a href="{{SupportUrl}}" style="color:#008545;font-weight:bold;text-decoration:none;">Destek Merkezi</a>
        </td>
      </tr>
    </tbody>
  </table>
</body>
</html>
""";
}
