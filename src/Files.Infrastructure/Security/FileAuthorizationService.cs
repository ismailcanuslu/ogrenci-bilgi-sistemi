using Files.Application.Abstractions.Security;
using Files.Infrastructure.Options;
using Microsoft.Extensions.Options;
using File = Files.Domain.Files.File;

namespace Files.Infrastructure.Security;

public sealed class FileAuthorizationService(
    IOptions<FileSecurityOptions> options)
    : IFileAuthorizationService
{
    public Task<bool> CanUploadAsync(
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        if (!options.Value.EnforceAuthorization)
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(userId.HasValue && userId.Value != Guid.Empty);
    }

    public Task<bool> CanReadAsync(
        File file,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        if (!options.Value.EnforceAuthorization)
        {
            return Task.FromResult(true);
        }

        if (options.Value.AllowAnonymousRead)
        {
            return Task.FromResult(true);
        }

        if (!file.OwnerId.HasValue)
        {
            return Task.FromResult(userId.HasValue && userId.Value != Guid.Empty);
        }

        return Task.FromResult(userId.HasValue && userId.Value == file.OwnerId.Value);
    }

    public Task<bool> CanDeleteAsync(
        File file,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        if (!options.Value.EnforceAuthorization)
        {
            return Task.FromResult(true);
        }

        if (!userId.HasValue || userId.Value == Guid.Empty)
        {
            return Task.FromResult(false);
        }

        if (!file.OwnerId.HasValue)
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(file.OwnerId.Value == userId.Value);
    }
}
