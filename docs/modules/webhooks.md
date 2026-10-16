# 🔗 Webhooks Module

[![Status](https://img.shields.io/badge/Status-Production--Ready-success.svg)](#)
[![Layer](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue.svg)](#)

The **Webhooks Module** enables real-time HTTP event dispatching to third-party integrations with **HMAC-SHA256 Signatures**, **Secret Rotation**, and **Resilient Exponential Backoff Retries**.

---

## 🌟 Key Features

- **🔐 Cryptographic HMAC-SHA256 Signatures**: Every payload is signed with a webhook secret (`X-Webhook-Signature` header) so consumers can verify authenticity.
- **🔄 Zero-Downtime Secret Rotation**: Rotate subscription secrets seamlessly.
- **⚡ Exponential Backoff Delivery Retries**: Automatically retries failed webhook deliveries up to configurable threshold.
- **📋 Delivery Audit History**: Complete log of HTTP status codes, latency, payload snippets, and response bodies.
- **🧪 Test Delivery Simulation**: Trigger sample test deliveries directly from the API.

---

## 📂 Module Architecture

```
Webhooks/
├── Webhooks.Domain/           # WebhookSubscription Aggregate, WebhookDeliveryLog Entity
├── Webhooks.Application/      # CQRS Handlers (CreateSubscription, RotateSecret, TestDelivery, GetLogs)
├── Webhooks.Infrastructure/   # WebhookDbContext, HmacSignatureGenerator, WebhookDispatcherService
└── Webhooks.Api/              # Minimal API Endpoints, Scalar Documentation
```

---

## 🔌 How to Use in Your Own Project

### 1. Register Dependencies in `Program.cs`
```csharp
using Webhooks.Application;
using Webhooks.Infrastructure;
using Webhooks.Api;

builder.Services.AddWebhooksApplication();
builder.Services.AddWebhooksInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapWebhooksEndpoints();
```

---

## 🚀 API Endpoints Overview

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/webhooks/subscriptions` | Register a new webhook target URL and event subscription |
| `GET` | `/api/v1/webhooks/subscriptions` | List all active webhook endpoints |
| `POST` | `/api/v1/webhooks/subscriptions/{id}/rotate-secret` | Generate and rotate subscription signing secret |
| `POST` | `/api/v1/webhooks/subscriptions/{id}/test` | Send a test ping payload to verify target endpoint |
| `GET` | `/api/v1/webhooks/deliveries` | Inspect delivery attempt logs, status codes, and latencies |
