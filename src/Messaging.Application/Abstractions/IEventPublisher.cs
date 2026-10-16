using Messaging.Domain;

namespace Messaging.Application.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : class, IIntegrationEvent;
}
