# 📝 Auditing Module

[![Status](https://img.shields.io/badge/Status-Production--Ready-success.svg)](#)
[![Layer](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue.svg)](#)

The **Auditing Module** provides centralized compliance and activity tracking across all microservices and modular monolith components. It logs who changed what, when, from which IP, and links entries to **OpenTelemetry Distributed Traces**.

---

## 🌟 Key Features

- **📊 Comprehensive Audit Records**: Logs user ID, action type, entity name, entity ID, previous state, new state, client IP address, and timestamp.
- **🔍 OpenTelemetry Correlation**: Integrates with `Activity.Current.TraceId` so audit records correlate 1:1 with Jaeger/OTEL distributed traces.
- **🛡️ Tamper-Resistant Storage**: Dedicated auditing database isolation prevents audit log tampering by standard service mutations.
- **⚡ High-Throughput Ingestion**: Efficient asynchronous queueing and persistence.

---

## 📂 Module Architecture

```
Auditing/
├── Auditing.Domain/           # AuditLog Entity, ActionType Enum
├── Auditing.Application/      # CQRS Handlers (RecordAuditLog, GetAuditLogs, SearchAuditLogs)
├── Auditing.Infrastructure/   # AuditingDbContext, TraceContextAccessor, EF Core Interceptor
└── Auditing.Api/              # Minimal API Endpoints, Scalar Documentation
```

---

## 🔌 How to Use in Your Own Project

### 1. Register Dependencies in `Program.cs`
```csharp
using Auditing.Application;
using Auditing.Infrastructure;
using Auditing.Api;

builder.Services.AddAuditingApplication();
builder.Services.AddAuditingInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapAuditingEndpoints();
```

---

## 🚀 API Endpoints Overview

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/auditing/logs` | Create a new structured audit log record |
| `GET` | `/api/v1/auditing/logs` | Query and filter audit trails by user, entity, date range, or trace ID |
| `GET` | `/api/v1/auditing/logs/{id}` | Inspect complete change diff for specific audit entry |
