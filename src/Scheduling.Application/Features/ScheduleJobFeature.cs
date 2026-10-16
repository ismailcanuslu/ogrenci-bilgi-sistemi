using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Scheduling.Application.Abstractions;

namespace Scheduling.Application.Features;

public static class ScheduleJobFeature
{
    public sealed record Command(
        string Name,
        string? Description = null,
        string? CronExpression = null,
        int? DelaySeconds = null,
        string? PayloadJson = null)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(Guid JobId);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }

    public sealed class Handler(ISchedulerService schedulerService)
        : ICommandHandler<Command, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            TimeSpan? delay = request.DelaySeconds.HasValue
                ? TimeSpan.FromSeconds(request.DelaySeconds.Value)
                : null;

            var jobId = await schedulerService.ScheduleJobAsync(
                request.Name,
                request.Description,
                request.CronExpression,
                delay,
                request.PayloadJson,
                cancellationToken);

            return ServiceResult<Response>.Created(new Response(jobId.Value));
        }
    }
}
