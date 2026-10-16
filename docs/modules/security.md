# 🛡️ Security Module

[![Status](https://img.shields.io/badge/Status-Production--Ready-success.svg)](#)
[![Layer](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue.svg)](#)

The **Security Module** is a standalone, enterprise-grade authentication and authorization engine built for modern applications. It supports **Password Authentication**, **FIDO2 / WebAuthn Passkeys (Biometric & Hardware Keys)**, **Asymmetric RSA JWT Tokens**, and **Active Session Management**.

---

## 🌟 Key Features

- **🔑 Asymmetric RSA JWT Authentication**: Issues access tokens signed with RSA private keys; validated with public keys without sharing secrets.
- **🛡️ FIDO2 / WebAuthn Passkeys**: Native passwordless authentication using Touch ID, Face ID, Windows Hello, or YubiKeys.
- **🔄 Sliding Refresh Tokens**: Cryptographically secure refresh token rotation with revoke capabilities.
- **📱 Active Session Management**: Track active devices, IP addresses, user agents, and revoke individual or all sessions remotely.
- **👥 Role & Permission Claims**: Granular claims-based access control.

---

## 📂 Module Architecture

```
Security/
├── Security.Domain/           # Entities (User, Session, PasskeyCredential), ValueObjects (Email, PhoneNumber), Primitives
├── Security.Application/      # CQRS Commands & Queries (Register, Login, Passkey Handlers, Token Refresh)
├── Security.Infrastructure/   # EF Core DbContext, Password Hasher, RSA Token Generator, Fido2 Adapter
└── Security.Api/              # Minimal API Endpoints, OpenAPI Scalar Metadata
```

---

## 🔌 How to Use in Your Own Project

You can extract the `Security.*` projects and plug them directly into your solution:

### 1. Register Dependencies in `Program.cs`
```csharp
using Security.Application;
using Security.Infrastructure;
using Security.Api;

// Application layer CQRS & MediatR handlers
builder.Services.AddSecurityApplication();

// Infrastructure layer (EF Core DbContext, Passkey services, Token generators)
builder.Services.AddSecurityInfrastructure(builder.Configuration);

var app = builder.Build();

// Map Security Endpoints
app.MapSecurityEndpoints();
```

---

## ⚙️ Configuration (`appsettings.json` or `.env`)

```json
{
  "ConnectionStrings": {
    "SecurityDatabase": "Server=localhost;Database=app_security;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Issuer": "universal-platform",
    "Audience": "universal-api",
    "AccessTokenLifetimeMinutes": 60,
    "PrivateKey": "-----BEGIN RSA PRIVATE KEY-----\nMIIEowIBAAKCAQEA0...",
    "PublicKey": "development-public-key"
  },
  "Fido2": {
    "Rpid": "localhost",
    "RpName": "Universal Platform",
    "Origin": "http://localhost:5000"
  }
}
```

---

## 🚀 API Endpoints Overview

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/auth/register` | Register a new user with email & password |
| `POST` | `/api/v1/auth/login` | Authenticate and obtain JWT access + refresh tokens |
| `POST` | `/api/v1/auth/refresh-token` | Exchange expired token for a new token pair |
| `POST` | `/api/v1/auth/passkey/register/begin` | Begin WebAuthn passkey registration ceremony |
| `POST` | `/api/v1/auth/passkey/register/complete` | Complete WebAuthn credential enrollment |
| `POST` | `/api/v1/auth/passkey/login/begin` | Begin WebAuthn authentication ceremony |
| `POST` | `/api/v1/auth/passkey/login/complete` | Finalize passkey authentication & return JWT |
| `POST` | `/api/v1/auth/sessions/revoke` | Revoke specific session or sign out all devices |
