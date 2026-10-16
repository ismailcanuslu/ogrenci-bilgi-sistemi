namespace Scheduling.Domain;

public interface IJob
{
    string Name { get; }

    Task ExecuteAsync(string? payloadJson, CancellationToken cancellationToken);
}
