# 🐳 Deployment, Environment Variables & Docker Guide

This guide details the step-by-step procedure to configure, provision, and deploy the platform in development, staging, or production environments using Docker Compose.

---

## 📋 Table of Contents
1. [Overview & Philosophy](#overview--philosophy)
2. [Step 1: Environment File Configuration (`.env`)](#step-1-environment-file-configuration-env)
3. [Step 2: Starting Infrastructure Services (`docker-compose.infra.yml`)](#step-2-starting-infrastructure-services-docker-composeinfrayml)
4. [Step 3: Starting Application APIs (`docker-compose.app.yml`)](#step-3-starting-application-apis-docker-composeappyml)
5. [Deployment Topologies (Modular Monolith vs Microservices)](#deployment-topologies)
6. [Accessing Services & Scalar Documentation](#accessing-services--scalar-documentation)
7. [Environment Variables Reference](#environment-variables-reference)

---

## 1. Overview & Philosophy

The project provides clean, decoupled Docker Compose definitions:
- **`docker-compose.infra.yml`**: Database engines (SQL Server, PostgreSQL), Redis cache, MinIO S3 Object Storage, and Mailpit SMTP.
- **`docker-compose.app.yml`**: Application services including the unified **Bootstrapper** (all-in-one Modular Monolith) and independent microservices.

---

## 2. Step 1: Environment File Configuration (`.env`)

In production or local development, always copy the template `.env.example` file to create your active `.env`:

```bash
# Navigate to the docker directory
cd docker

# Copy the example environment file
cp .env.example .env
```

> [!IMPORTANT]
> Never commit `.env` into source control. Always verify that `.env` contains strong passwords for databases, your RSA private key for JWTs, and your production SMS credentials.

---

## 3. Step 2: Starting Infrastructure Services (`docker-compose.infra.yml`)

Provision all underlying data stores, caches, and emulators:

```bash
docker compose -f docker/docker-compose.infra.yml up -d
```

### What this starts:
| Container | Service | Port | Description |
|---|---|---|---|
| `obs-sqlserver` | Microsoft SQL Server 2022 | `1433` | Primary relational database |
| `obs-postgres` | PostgreSQL 16 Alpine | `5432` | Secondary relational database option |
| `obs-redis` | Redis Alpine | `6379` | L2 distributed cache & lock provider |
| `obs-minio` | MinIO Object Store | `9000` / `9001` | S3-compatible file storage & web console |
| `obs-mailpit` | Mailpit SMTP Server | `1025` / `8025` | Email capture engine & web inbox |

Verify health status:
```bash
docker compose -f docker/docker-compose.infra.yml ps
```

---

## 4. Step 3: Starting Application APIs (`docker-compose.app.yml`)

Once infrastructure services are healthy, launch the application containers:

```bash
docker compose -f docker/docker-compose.app.yml up -d
```

To run only the unified all-in-one **Bootstrapper** (Modular Monolith):
```bash
docker compose -f docker/docker-compose.app.yml up -d obs-bootstrapper
```

---

## 5. Deployment Topologies

You have full freedom in how you deploy this architecture:

### Option A: Modular Monolith (Recommended for 95% of use cases)
Run `obs-bootstrapper`. It loads all modules into a single, high-performance process, eliminating network latency and operational overhead while maintaining strict DDD boundaries.
- **API URL**: `http://localhost:5000`
- **Scalar UI**: `http://localhost:5000/scalar/v1`

### Option B: Distributed Microservices
Run the individual standalone services:
- Security API: `http://localhost:5001/scalar/v1`
- Files API: `http://localhost:5002/scalar/v1`
- Notifications API: `http://localhost:5003/scalar/v1`
- Translations API: `http://localhost:5004/scalar/v1`
- Auditing API: `http://localhost:5005/scalar/v1`
- Messaging API: `http://localhost:5006/scalar/v1`
- Scheduling API: `http://localhost:5007/scalar/v1`
- Webhooks API: `http://localhost:5008/scalar/v1`
- Import & Export API: `http://localhost:5009/scalar/v1`

---

## 6. Accessing Services & Scalar Documentation

| Interface | URL | Credentials (Defaults) |
|---|---|---|
| **Unified Scalar API UI** | `http://localhost:5000/scalar/v1` | No auth required for documentation |
| **Mailpit Web Inbox** | `http://localhost:8025` | N/A |
| **MinIO Console** | `http://localhost:9001` | `minioadmin` / `minioadmin` |

---

## 7. Environment Variables Reference

| Variable | Default | Purpose |
|---|---|---|
| `COMPOSE_PROJECT_NAME` | `obs-platform` | Docker Compose network/volume prefix |
| `MSSQL_SA_PASSWORD` | `YourStrong@Password123!` | SQL Server admin password |
| `REDIS_PORT` | `6379` | Redis port |
| `MINIO_ROOT_USER` | `minioadmin` | S3 Access Key |
| `MINIO_ROOT_PASSWORD` | `minioadmin` | S3 Secret Key |
| `MINIO_BUCKET_NAME` | `obs-files` | Target S3 bucket |
| `SMTP_FROM_EMAIL` | `noreply@example.com` | Email sender address |
| `BRANDING_COMPANY_NAME`| `OBS Platform` | Email templates branding header |
| `BRANDING_LOGO_URL` | `https://example.com/logo.png`| Email logo image |
| `BRANDING_PRIMARY_COLOR`| `#6166BE` | Email template primary accent color |
| `SMS_PROVIDER` | `Console` (or `Http`) | SMS provider engine |
| `JWT_ISSUER` | `obs-platform` | Token issuer claim |
| `JWT_AUDIENCE` | `obs-api` | Token audience claim |
| `OTEL_EXPORTER_OTLP_ENDPOINT`| `http://obs-jaeger:4317` | OpenTelemetry collector |
