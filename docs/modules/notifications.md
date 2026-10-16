# 📬 Notifications Module

[![Status](https://img.shields.io/badge/Status-Production--Ready-success.svg)](#)
[![Layer](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue.svg)](#)

The **Notifications Module** provides an omnichannel delivery system supporting **Responsive Templated Emails**, **One-Time Passcode (OTP) Verification**, **Action Buttons**, **SMS Gateways**, and **Broadcast Messaging** with built-in retry and status lifecycle tracking.

---

## 🌟 Key Features

- **📧 Responsive HTML Email Templates**: Pre-configured, beautiful card-based templates supporting both OTP codes and call-to-action buttons.
- **🎨 Brand Customization**: Externalized configuration for company logos, brand colors, support links, and addresses.
- **📱 Multi-Provider SMS Engine**: Send verification SMS messages or system maintenance alerts via HTTP SMS API gateways with automatic console fallback for development.
- **🔄 Resilient Retry & Status Lifecycle**: Track messages through `Pending`, `Sent`, `Failed`, and `Cancelled` states with automatic retry handling.
- **🛡️ Value Object Validation**: Strict domain validation for emails, phone numbers, and message bodies.

---

## 📂 Module Architecture

```
Notifications/
├── Notifications.Domain/           # Notification Aggregate, ValueObjects (EmailAddress, PhoneNumber, Body, Subject), Enums
├── Notifications.Application/      # CQRS Handlers (SendEmail, SendTemplatedEmail, SendSms, SendBroadcastSms, Retry, Cancel)
├── Notifications.Infrastructure/   # SmtpEmailSender, HttpSmsSender, TemplateEngine, EF Core DbContext
└── Notifications.Api/              # Minimal API Endpoints, Scalar Documentation
```

---

## 🔌 How to Use in Your Own Project

### 1. Register Dependencies in `Program.cs`
```csharp
using Notifications.Application;
using Notifications.Infrastructure;
using Notifications.Api;

// Register Application & Infrastructure
builder.Services.AddNotificationsApplication();
builder.Services.AddNotificationsInfrastructure(builder.Configuration);

var app = builder.Build();

// Map Notification Endpoints
app.MapNotificationsEndpoints();
```

---

## ⚙️ Configuration (`appsettings.json` or `.env`)

```json
{
  "ConnectionStrings": {
    "NotificationsDatabase": "Server=localhost;Database=app_notifications;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
  },
  "Notifications": {
    "Provider": "Smtp",
    "Smtp": {
      "Host": "localhost",
      "Port": 1025,
      "EnableSsl": false,
      "FromEmail": "noreply@example.com",
      "FromName": "Your Platform"
    },
    "Branding": {
      "CompanyName": "Acme Corporation",
      "LogoUrl": "https://example.com/logo.png",
      "SupportUrl": "https://support.example.com",
      "SupportEmail": "support@example.com",
      "CompanyAddress": "Silicon Valley, CA, USA",
      "PrimaryColor": "#6166BE",
      "AccentColor": "#F0F0F9",
      "DefaultTheme": "ModernCard"
    },
    "Sms": {
      "Provider": "Http",
      "SenderHeader": "ACMEALERT",
      "EndpointUrl": "https://api.sms-gateway.example.com/v1/send",
      "ApiKey": "secret-api-key",
      "ApiSecret": "optional-secret"
    }
  }
}
```

---

## 🚀 API Endpoints Overview

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/notifications/email` | Send a raw email with custom subject and body |
| `POST` | `/api/v1/notifications/email/templated` | Send a branded email using a template (e.g. OTP code or button link) |
| `POST` | `/api/v1/notifications/sms` | Send an OTP or standard verification SMS |
| `POST` | `/api/v1/notifications/sms/broadcast` | Send a freeform system or maintenance broadcast SMS |
| `GET` | `/api/v1/notifications/{id}/status` | Check delivery status, retry counts, or error logs |
| `POST` | `/api/v1/notifications/{id}/retry` | Re-queue a failed notification for delivery |
| `POST` | `/api/v1/notifications/{id}/cancel` | Cancel a pending notification |
| `GET` | `/api/v1/notifications/history` | Retrieve historical logs for audit and tracking |
