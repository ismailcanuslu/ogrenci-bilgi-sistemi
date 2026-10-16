using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Auditing.Application.Abstractions;
using Auditing.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Auditing.Infrastructure.Services;

public sealed class AuditService(
    IAuditLogRepository repository,
    IHttpContextAccessor? httpContextAccessor = null,
    ILogger<AuditService>? logger = null)
    : IAuditService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task RecordAsync(
        string action,
        string module,
        string resource,
        string? resourceId = null,
        object? oldValues = null,
        object? newValues = null,
        bool isSuccess = true,
        string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var httpContext = httpContextAccessor?.HttpContext;
            var user = httpContext?.User;

            Guid? userId = null;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("sub")?.Value;

            if (!string.IsNullOrWhiteSpace(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedGuid))
            {
                userId = parsedGuid;
            }

            var userName = user?.FindFirst(ClaimTypes.Name)?.Value
                ?? user?.FindFirst("name")?.Value
                ?? user?.FindFirst(ClaimTypes.Email)?.Value
                ?? user?.FindFirst("email")?.Value;

            var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = httpContext?.Request?.Headers.UserAgent.ToString();

            var activity = Activity.Current;
            var traceId = activity?.TraceId.ToString() ?? httpContext?.TraceIdentifier;
            var spanId = activity?.SpanId.ToString();

            string? oldValuesJson = oldValues is not null
                ? (oldValues is string s1 ? s1 : JsonSerializer.Serialize(oldValues, JsonOptions))
                : null;

            string? newValuesJson = newValues is not null
                ? (newValues is string s2 ? s2 : JsonSerializer.Serialize(newValues, JsonOptions))
                : null;

            var log = AuditLog.Create(
                userId: userId,
                userName: userName,
                action: action,
                module: module,
                resource: resource,
                resourceId: resourceId,
                oldValuesJson: oldValuesJson,
                newValuesJson: newValuesJson,
                ipAddress: ipAddress,
                userAgent: userAgent,
                traceId: traceId,
                spanId: spanId,
                timestamp: DateTimeOffset.UtcNow,
                isSuccess: isSuccess,
                errorMessage: errorMessage);

            await repository.AddAsync(log, cancellationToken);

            if (activity is not null)
            {
                activity.AddEvent(new ActivityEvent("AuditLogRecorded", tags: new ActivityTagsCollection
                {
                    ["audit.action"] = action,
                    ["audit.module"] = module,
                    ["audit.resource"] = resource,
                    ["audit.success"] = isSuccess
                }));
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to record audit log for action {Action} in module {Module}", action, module);
        }
    }
}
