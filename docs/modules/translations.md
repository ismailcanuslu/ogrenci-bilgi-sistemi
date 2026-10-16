# 🌐 Translations Module

[![Status](https://img.shields.io/badge/Status-Production--Ready-success.svg)](#)
[![Layer](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue.svg)](#)

The **Translations Module** delivers a dynamic, high-performance localization catalog for multi-lingual platforms. It supports **Turkish (TR)**, **English (EN)**, **German (DE)**, and **Russian (RU)** with automatic fallback logic.

---

## 🌟 Key Features

- **🌍 Multi-Language Catalog**: Out-of-the-box support for TR, EN, DE, and RU languages.
- **🔄 Intelligent Fallback Mechanism**: If a requested language key is missing in German or Russian, it automatically falls back to Turkish (default).
- **⚡ In-Memory & Redis Cached Translations**: High-throughput key lookups suitable for low-latency API responses.
- **🛠️ Dynamic Catalog Management**: Add or update translation strings at runtime without restarting services.

---

## 📂 Module Architecture

```
Translations/
├── Translations.Domain/           # LanguageCode Enum, Translation Key Definitions
├── Translations.Application/      # CQRS Handlers (GetTranslation, GetTranslationsCatalog, UpdateTranslation)
├── Translations.Infrastructure/   # TranslationRepository, Hybrid Caching Adapter
└── Translations.Api/              # Minimal API Endpoints, Scalar Documentation
```

---

## 🔌 How to Use in Your Own Project

### 1. Register Dependencies in `Program.cs`
```csharp
using Translations.Application;
using Translations.Infrastructure;
using Translations.Api;

builder.Services.AddTranslationsApplication();
builder.Services.AddTranslationsInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapTranslationsEndpoints();
```

---

## 🚀 API Endpoints Overview

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/translations/{lang}/{key}` | Retrieve localized string for specific key (with fallback) |
| `GET` | `/api/v1/translations/{lang}` | Retrieve complete dictionary for language code |
| `GET` | `/api/v1/translations/catalog` | Retrieve all available translations and supported languages |
