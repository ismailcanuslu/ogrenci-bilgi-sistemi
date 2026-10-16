using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Messaging.Application.Abstractions;

namespace Messaging.Application.Features;

public static class GetOutboxMessages
{
    public sealed record Query(
        int Page = 1,
        int PageSize = 20,
        bool? ProcessedOnly = null)
        : IQuery<ServiceResult<Response>>;

    public sealed record Response(
        IReadOnlyList<ItemDto> Items,
        int TotalCount,
        int Page,
        int PageSize);

    public sealed record ItemDto(
        Guid Id,
        DateTimeOffset OccurredOnUtc,
        string Type,
        string PayloadJson,
        DateTimeOffset? ProcessedOnUtc,
        string? Error,
        int RetryCount,
        string? TraceId,
        string? SpanId,
        bool IsProcessed);

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }
    }

    public sealed class Handler(IOutboxRepository repository)
        : IQueryHandler<Query, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await repository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.ProcessedOnly,
                cancellationToken);

            var dtos = items.Select(x => new ItemDto(
                x.Id.Value,
                x.OccurredOnUtc,
                x.Type,
                x.PayloadJson,
                x.ProcessedOnUtc,
                x.Error,
                x.RetryCount,
                x.TraceId,
                x.SpanId,
                x.IsProcessed))
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
