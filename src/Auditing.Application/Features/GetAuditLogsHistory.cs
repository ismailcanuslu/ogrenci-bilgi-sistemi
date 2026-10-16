using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Auditing.Application.Abstractions;

namespace Auditing.Application.Features;

public static class GetAuditLogsHistory
{
    public sealed record Query(
        int Page = 1,
        int PageSize = 20,
        string? Module = null,
        string? Action = null,
        Guid? UserId = null,
        string? Resource = null,
        DateTimeOffset? From = null,
        DateTimeOffset? To = null)
        : IQuery<ServiceResult<Response>>;

    public sealed record Response(
        IReadOnlyList<ItemDto> Items,
        int TotalCount,
        int Page,
        int PageSize);

    public sealed record ItemDto(
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
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);
        }
    }

    public sealed class Handler(IAuditLogRepository repository)
        : IQueryHandler<Query, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await repository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.Module,
                request.Action,
                request.UserId,
                request.Resource,
                request.From,
                request.To,
                cancellationToken);

            var dtos = items.Select(x => new ItemDto(
                x.Id.Value,
                x.UserId,
                x.UserName,
                x.Action,
                x.Module,
                x.Resource,
                x.ResourceId,
                x.OldValuesJson,
                x.NewValuesJson,
                x.IpAddress,
                x.UserAgent,
                x.TraceId,
                x.SpanId,
                x.Timestamp,
                x.IsSuccess,
                x.ErrorMessage))
                .ToList();

            var response = new Response(
                dtos,
                totalCount,
                request.Page,
                request.PageSize);

            return ServiceResult<Response>.Ok(response);
        }
    }
}
