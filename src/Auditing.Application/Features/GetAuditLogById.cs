using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Auditing.Application.Abstractions;
using Auditing.Domain;

namespace Auditing.Application.Features;

public static class GetAuditLogById
{
    public sealed record Query(Guid Id) : IQuery<ServiceResult<Response>>;

    public sealed record Response(
        Guid Id,
        Guid? UserId,
        string? UserName,
        string Action,
        string Module,
        string Resource,
        string? ResourceId,
        string? OldValuesJson,
        string? NewValuesJson,
        string? IpAddress,
        string? UserAgent,
        string? TraceId,
        string? SpanId,
        DateTimeOffset Timestamp,
        bool IsSuccess,
        string? ErrorMessage);

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(IAuditLogRepository repository)
        : IQueryHandler<Query, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var auditLog = await repository.GetByIdAsync(
                new AuditLogId(request.Id),
                cancellationToken);

            if (auditLog is null)
            {
                return ServiceResult<Response>.NotFound(new Error("Auditing.NotFound", ErrorType.NotFound));
            }

            var response = new Response(
                auditLog.Id.Value,
                auditLog.UserId,
                auditLog.UserName,
                auditLog.Action,
                auditLog.Module,
                auditLog.Resource,
                auditLog.ResourceId,
                auditLog.OldValuesJson,
                auditLog.NewValuesJson,
                auditLog.IpAddress,
                auditLog.UserAgent,
                auditLog.TraceId,
                auditLog.SpanId,
                auditLog.Timestamp,
                auditLog.IsSuccess,
                auditLog.ErrorMessage);

            return ServiceResult<Response>.Ok(response);
        }
    }
}
