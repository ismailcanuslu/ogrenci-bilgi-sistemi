# 📨 Messaging Module

[![Status](https://img.shields.io/badge/Status-Production--Ready-success.svg)](#)
[![Layer](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue.svg)](#)

The **Messaging Module** implements the **Transactional Outbox Pattern** to ensure guaranteed, at-least-once message delivery between domain services and external message brokers (RabbitMQ / MassTransit).

---

## 🌟 Key Features

- **📬 Transactional Outbox Pattern**: Persist domain events and database mutations within the exact same database transaction, eliminating dual-write failure modes.
- **🔄 Asynchronous Background Publisher**: Dedicated worker polls pending outbox records and publishes them safely to the message bus.
- **🛡️ Idempotent Message Handling**: Prevents duplicate message processing with idempotency keys.
- **⚡ Exponential Backoff Retries**: Failed deliveries are retried gracefully with error reason recording.

---

## 📂 Module Architecture

```
Messaging/
├── Messaging.Domain/           # OutboxMessage Entity, OutboxStatus Enum
├── Messaging.Application/      # CQRS Handlers (PublishOutboxMessage, ProcessOutboxMessages, GetOutboxLogs)
├── Messaging.Infrastructure/   # OutboxDbContext, BackgroundOutboxProcessor, RabbitMqPublisher
└── Messaging.Api/              # Minimal API Endpoints, Scalar Documentation
```

---

## 🔌 How to Use in Your Own Project

### 1. Register Dependencies in `Program.cs`
```csharp
using Messaging.Application;
using Messaging.Infrastructure;
using Messaging.Api;

builder.Services.AddMessagingApplication();
builder.Services.AddMessagingInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapMessagingEndpoints();
```

---

## 🚀 API Endpoints Overview

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/messaging/outbox` | Enqueue a message into the transactional outbox |
| `GET` | `/api/v1/messaging/outbox` | View outbox queue status, pending messages, and retry counters |
| `POST` | `/api/v1/messaging/outbox/process` | Trigger immediate on-demand outbox processing |
