# ⏰ Scheduling Module

[![Status](https://img.shields.io/badge/Status-Production--Ready-success.svg)](#)
[![Layer](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue.svg)](#)

The **Scheduling Module** provides an enterprise background job orchestration engine supporting **Cron-based recurring schedules**, **one-shot delayed tasks**, and **manual job triggers** with persistent execution history.

---

## 🌟 Key Features

- **⏱️ Standard 5-Field Cron Scheduling**: Configure recurring jobs with standard cron expressions (e.g. `*/5 * * * *`).
- **⏳ One-Shot Delayed Jobs**: Schedule tasks to execute at a precise future timestamp.
- **⚡ Manual On-Demand Triggers**: Trigger any job immediately for testing or maintenance.
- **🛑 Cancellation & Status Tracking**: Cancel active schedules and inspect historic run duration and error traces.

---

## 📂 Module Architecture

```
Scheduling/
├── Scheduling.Domain/           # JobDefinition Aggregate, JobExecutionLog, JobStatus Enum
├── Scheduling.Application/      # CQRS Handlers (ScheduleJob, TriggerJob, CancelJob, GetJobs)
├── Scheduling.Infrastructure/   # SchedulingDbContext, BackgroundSchedulerEngine
└── Scheduling.Api/              # Minimal API Endpoints, Scalar Documentation
```

---

## 🔌 How to Use in Your Own Project

### 1. Register Dependencies in `Program.cs`
```csharp
using Scheduling.Application;
using Scheduling.Infrastructure;
using Scheduling.Api;

builder.Services.AddSchedulingApplication();
builder.Services.AddSchedulingInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapSchedulingEndpoints();
```

---

## 🚀 API Endpoints Overview

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/scheduling/jobs` | Create a new recurring (cron) or one-shot delayed background job |
| `GET` | `/api/v1/scheduling/jobs` | Retrieve all registered jobs and their next scheduled execution |
| `POST` | `/api/v1/scheduling/jobs/{id}/trigger` | Manually trigger immediate execution of a job |
| `POST` | `/api/v1/scheduling/jobs/{id}/cancel` | Cancel an active job schedule |
| `GET` | `/api/v1/scheduling/jobs/{id}/logs` | Inspect execution logs and run durations |
