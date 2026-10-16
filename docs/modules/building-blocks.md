# 🧱 BuildingBlocks (Shared Kernel & Core Framework)

[![Status](https://img.shields.io/badge/Status-Production--Ready-success.svg)](#)
[![Layer](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-blue.svg)](#)

**BuildingBlocks** forms the infrastructural and architectural bedrock of the entire solution. It encapsulates the shared domain primitives, application CQRS behaviors, Polly v8 resilience pipelines, hybrid caching, and modern OpenAPI / Scalar UI integrations.

---

## 🌟 Key Features

- **🏛️ Domain Primitives (`BuildingBlocks.Domain`)**: Base `Entity<T>`, `AggregateRoot<T>`, `ValueObject`, `IDomainEvent`, and `DomainException`.
- **⚙️ Application CQRS & Pipeline Behaviors (`BuildingBlocks.Application`)**: `Result<T>` pattern, `ICommand`, `IQuery`, MediatR validation pipeline, and performance behaviors.
- **🛡️ Polly v8 Resilience (`BuildingBlocks.Infrastructure`)**: Circuit breakers, retry with jitter, timeout, and fallback policies configured via the modern Polly v8 strategy builder.
- **⚡ Hybrid Caching (`ICacheService`)**: Transparent L1 In-Memory + L2 Distributed Redis caching with stampede prevention.
- **📖 OpenAPI & Scalar UI (`BuildingBlocks.Api`)**: Instant, dark-themed, interactive API documentation hosted natively at `/scalar/v1` for all modules.

---

## 📂 Architecture Breakdown

```
BuildingBlocks/
├── BuildingBlocks.Domain/          # Entity, AggregateRoot, ValueObject, DomainEvent
├── BuildingBlocks.Application/     # Result<T>, Error, MediatR Behaviors
├── BuildingBlocks.Infrastructure/  # ICacheService (Redis), Polly Resilience, OpenTelemetry
└── BuildingBlocks.Api/             # EndpointRouteBuilderExtensions, ScalarExtensions (/scalar/v1)
```

---

## 🔌 How to Use in Your Own Project

```csharp
using BuildingBlocks.Api;
using BuildingBlocks.Api.OpenApi;
using BuildingBlocks.Infrastructure.Caching;
using BuildingBlocks.Infrastructure.Resilience;

// Add Hybrid Caching
builder.Services.AddHybridCaching(builder.Configuration);

// Add Polly v8 Resilience Pipelines
builder.Services.AddResiliencePipelines();

// Add Scalar & OpenAPI Documentation
builder.Services.AddOpenApiWithScalar();

var app = builder.Build();

// Map Scalar UI at /scalar/v1
app.MapOpenApiWithScalar();
```
