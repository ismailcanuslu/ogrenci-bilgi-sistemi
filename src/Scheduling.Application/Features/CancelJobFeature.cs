using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using Scheduling.Application.Abstractions;
using Scheduling.Domain;

namespace Scheduling.Application.Features;

public static class CancelJobFeature
{
    public sealed record Command(Guid JobId) : ICommand<ServiceResult>;

    public sealed class Handler(ISchedulerService schedulerService)
        : ICommandHandler<Command, ServiceResult>
    {
        public async Task<ServiceResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var success = await schedulerService.CancelJobAsync(new JobId(request.JobId), cancellationToken);
            if (!success)
            {
                return ServiceResult.NotFound(new Error("Scheduling.JobNotFound", ErrorType.NotFound));
            }

            return ServiceResult.NoContent();
        }
    }
}
