# 📁 Files Module

[![Status](https://img.shields.io/badge/Status-Production--Ready-success.svg)](#)
[![Layer](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue.svg)](#)

The **Files Module** handles cloud and on-premise object storage with built-in **AWS S3 / MinIO compatibility**, **Local Disk storage**, **SHA-256 Checksums**, **Deduplication**, and **Granular Access Control**.

---

## 🌟 Key Features

- **☁️ Multi-Provider Storage**: Switch seamlessly between AWS S3 / MinIO and Local Disk Storage via configuration.
- **🔒 SHA-256 Cryptographic Checksums**: Automatically computes file hashes for integrity verification and deduplication.
- **🛡️ Secure Access Control**: Optional role-based and ownership checks to prevent unauthorized file downloads.
- **📦 Streaming Upload & Download**: Memory-efficient streaming of large payloads without memory buffer bottlenecks.
- **📋 Rich Metadata Tracking**: Tracks content type, size, upload timestamp, ownership, and original file name.

---

## 📂 Module Architecture

```
Files/
├── Files.Domain/           # File Aggregate, ValueObjects (FileName, FileHash, FileSize, ContentType, StorageKey)
├── Files.Application/      # CQRS Handlers (UploadFile, DownloadFile, DeleteFile, GetMetadata)
├── Files.Infrastructure/   # S3StorageService, PhysicalDiskStorageService, FileValidator, EF Core DbContext
└── Files.Api/              # Minimal API Endpoints, File Upload & Download Endpoints
```

---

## 🔌 How to Use in Your Own Project

### 1. Register Dependencies in `Program.cs`
```csharp
using Files.Application;
using Files.Infrastructure;
using Files.Api;

builder.Services.AddFilesApplication();
builder.Services.AddFilesInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapFilesEndpoints();
```

---

## ⚙️ Configuration (`appsettings.json` or `.env`)

```json
{
  "ConnectionStrings": {
    "FilesDatabase": "Server=localhost;Database=app_files;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
  },
  "Files": {
    "Storage": {
      "Provider": "S3",
      "BasePath": "uploads",
      "S3": {
        "ServiceUrl": "http://localhost:9000",
        "BucketName": "app-files",
        "AccessKey": "minioadmin",
        "SecretKey": "minioadmin",
        "Region": "eu-central-1",
        "ForcePathStyle": true
      }
    },
    "Validation": {
      "MaxFileSizeInBytes": 52428800
    },
    "Security": {
      "EnforceAuthorization": false,
      "AllowAnonymousRead": false,
      "AdminRoleName": "Admin"
    }
  }
}
```

---

## 🚀 API Endpoints Overview

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/files/upload` | Multipart file upload with SHA-256 calculation |
| `GET` | `/api/v1/files/{id}/download` | Stream file content with correct Content-Type header |
| `GET` | `/api/v1/files/{id}/metadata` | Retrieve file size, hash, original name, and metadata |
| `DELETE` | `/api/v1/files/{id}` | Permanently delete file from database and object store |
