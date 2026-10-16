namespace Messaging.Application.Abstractions;

public interface IMessageBus
{
    ValueTask PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;

    IDisposable Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : class;
}
