using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Scheduling.Application.Abstractions;
using Scheduling.Domain;

namespace Scheduling.Application.Features;

public static class GetScheduledJobs
{
    public sealed record Query(
        int Page = 1,
        int PageSize = 20,
        JobStatus? Status = null)
        : IQuery<ServiceResult<Response>>;

    public sealed record Response(
        IReadOnlyList<ItemDto> Items,
        int TotalCount,
        int Page,
        int PageSize);

    public sealed record ItemDto(
        Guid Id,
        string Name,
        string? Description,
        string? CronExpression,
        string? PayloadJson,
        string Status,
        DateTimeOffset? NextExecutionTimeUtc,
        DateTimeOffset? LastExecutedTimeUtc,
        int RetryCount,
        int MaxRetries,
        string? ErrorMessage,
        DateTimeOffset CreatedAtUtc);

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }
    }

    public sealed class Handler(IJobRepository repository)
        : IQueryHandler<Query, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await repository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.Status,
                cancellationToken);

            var dtos = items.Select(x => new ItemDto(
                x.Id.Value,
                x.Name,
                x.Description,
                x.CronExpression,
                x.PayloadJson,
                x.Status.ToString(),
                x.NextExecutionTimeUtc,
                x.LastExecutedTimeUtc,
                x.RetryCount,
                x.MaxRetries,
                x.ErrorMessage,
                x.CreatedAtUtc))
                .ToList();

            var response = new Response(dtos, totalCount, request.Page, request.PageSize);
            return ServiceResult<Response>.Ok(response);
        }
    }
}
