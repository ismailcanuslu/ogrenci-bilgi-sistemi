using Notifications.Domain.ValueObjects;

namespace Notifications.Application.Abstractions.Infrastructure;

public interface ISmsSender
{
    Task SendSmsAsync(
        PhoneNumber to,
        Body body,
        CancellationToken cancellationToken = default);
}
