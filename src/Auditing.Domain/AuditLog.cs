using BuildingBlocks.Domain.Primitives;

namespace Auditing.Domain;

public sealed class AuditLog : Entity<AuditLogId>
{
    private AuditLog(
        AuditLogId id,
        Guid? userId,
        string? userName,
        string action,
        string module,
        string resource,
        string? resourceId,
        string? oldValuesJson,
        string? newValuesJson,
        string? ipAddress,
        string? userAgent,
        string? traceId,
        string? spanId,
        DateTimeOffset timestamp,
        bool isSuccess,
        string? errorMessage)
        : base(id)
    {
        UserId = userId;
        UserName = userName;
        Action = action;
        Module = module;
        Resource = resource;
        ResourceId = resourceId;
        OldValuesJson = oldValuesJson;
        NewValuesJson = newValuesJson;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        TraceId = traceId;
        SpanId = spanId;
        Timestamp = timestamp;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    private AuditLog()
    {
    }

    public Guid? UserId { get; private set; }

    public string? UserName { get; private set; }

    public string Action { get; private set; } = default!;

    public string Module { get; private set; } = default!;

    public string Resource { get; private set; } = default!;

    public string? ResourceId { get; private set; }

    public string? OldValuesJson { get; private set; }

    public string? NewValuesJson { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public string? TraceId { get; private set; }

    public string? SpanId { get; private set; }

    public DateTimeOffset Timestamp { get; private set; }

    public bool IsSuccess { get; private set; }

    public string? ErrorMessage { get; private set; }

    public static AuditLog Create(
        Guid? userId,
        string? userName,
        string action,
        string module,
        string resource,
        string? resourceId,
        string? oldValuesJson,
        string? newValuesJson,
        string? ipAddress,
        string? userAgent,
        string? traceId,
        string? spanId,
        DateTimeOffset timestamp,
        bool isSuccess = true,
        string? errorMessage = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        ArgumentException.ThrowIfNullOrWhiteSpace(module);
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);

        return new AuditLog(
            AuditLogId.New(),
            userId,
            userName,
            action,
            module,
            resource,
            resourceId,
            oldValuesJson,
            newValuesJson,
            ipAddress,
            userAgent,
            traceId,
            spanId,
            timestamp,
            isSuccess,
            errorMessage);
    }
}
