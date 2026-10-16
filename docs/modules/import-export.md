# 📊 Import & Export Module

[![Status](https://img.shields.io/badge/Status-Production--Ready-success.svg)](#)
[![Layer](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue.svg)](#)

The **Import & Export Module** provides asynchronous, fault-tolerant background data ingestion and extraction for **CSV and Excel (XLSX)** datasets.

---

## 🌟 Key Features

- **⚡ Asynchronous Background Processing**: Large files are processed in the background without blocking HTTP threads.
- **📈 Real-Time Progress Tracking**: Track processing states (`Queued`, `Processing`, `Completed`, `Failed`) with percentage and row counter tracking.
- **🛡️ Row-Level Validation & Error Reporting**: Detailed error manifests pinpoint exact row numbers and validation failure reasons.
- **📤 Data Export Engine**: Stream filtered domain collections into downloadable CSV or Excel formats.

---

## 📂 Module Architecture

```
ImportExport/
├── ImportExport.Domain/           # ImportJob Aggregate, ExportJob Aggregate, JobStatus Enum
├── ImportExport.Application/      # CQRS Handlers (CreateImportJob, GetImportJobs, CreateExportJob)
├── ImportExport.Infrastructure/   # ImportExportDbContext, CsvBatchProcessor, ExcelBatchProcessor
└── ImportExport.Api/              # Minimal API Endpoints, Scalar Documentation
```

---

## 🔌 How to Use in Your Own Project

### 1. Register Dependencies in `Program.cs`
```csharp
using ImportExport.Application;
using ImportExport.Infrastructure;
using ImportExport.Api;

builder.Services.AddImportExportApplication();
builder.Services.AddImportExportInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapImportExportEndpoints();
```

---

## 🚀 API Endpoints Overview

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/import-export/import` | Submit a CSV/Excel file for background processing |
| `GET` | `/api/v1/import-export/jobs/{id}` | Inspect progress percentage, processed rows, and failure errors |
| `GET` | `/api/v1/import-export/jobs` | List recent import and export jobs |
| `POST` | `/api/v1/import-export/export` | Request asynchronous dataset export |
