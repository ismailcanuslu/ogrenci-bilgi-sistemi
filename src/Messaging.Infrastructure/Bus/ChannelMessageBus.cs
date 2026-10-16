using System.Collections.Concurrent;
using System.Diagnostics;
using Messaging.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Messaging.Infrastructure.Bus;

public sealed class ChannelMessageBus(ILogger<ChannelMessageBus>? logger = null) : IMessageBus
{
    private readonly ConcurrentDictionary<Type, List<Func<object, CancellationToken, Task>>> _subscribers = new();
    private readonly object _lock = new();

    public async ValueTask PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(message);

        var messageType = message.GetType();
        List<Func<object, CancellationToken, Task>> handlersToRun;

        lock (_lock)
        {
            if (!_subscribers.TryGetValue(messageType, out var handlers) || handlers.Count == 0)
            {
                logger?.LogDebug("No subscribers found for message type {Type}", messageType.Name);
                return;
            }

            handlersToRun = [.. handlers];
        }

        var activity = Activity.Current;
        activity?.AddEvent(new ActivityEvent("MessageBusPublishing", tags: new ActivityTagsCollection
        {
            ["messaging.type"] = messageType.FullName,
            ["messaging.handler_count"] = handlersToRun.Count
        }));

        foreach (var handler in handlersToRun)
        {
            try
            {
                await handler(message, cancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error occurred while executing message handler for type {Type}", messageType.Name);
            }
        }
    }

    public IDisposable Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : class
    {
        ArgumentNullException.ThrowIfNull(handler);

        var messageType = typeof(T);

        Task Wrapper(object msg, CancellationToken ct) => handler((T)msg, ct);

        lock (_lock)
        {
            var list = _subscribers.GetOrAdd(messageType, _ => []);
            list.Add(Wrapper);
        }

        return new SubscriptionToken(() =>
        {
            lock (_lock)
            {
                if (_subscribers.TryGetValue(messageType, out var list))
                {
                    list.Remove(Wrapper);
                }
            }
        });
    }

    private sealed class SubscriptionToken(Action onDispose) : IDisposable
    {
        private Action? _onDispose = onDispose;

        public void Dispose()
        {
            Interlocked.Exchange(ref _onDispose, null)?.Invoke();
        }
    }
}
