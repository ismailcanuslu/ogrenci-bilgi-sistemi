using File = Files.Domain.Files.File;

namespace Files.Application.Abstractions.Security;

public interface IFileAuthorizationService
{
    Task<bool> CanUploadAsync(
        Guid? userId,
        CancellationToken cancellationToken = default);

    Task<bool> CanReadAsync(
        File file,
        Guid? userId,
        CancellationToken cancellationToken = default);

    Task<bool> CanDeleteAsync(
        File file,
        Guid? userId,
        CancellationToken cancellationToken = default);
}
