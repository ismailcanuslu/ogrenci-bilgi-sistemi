using BuildingBlocks.Application.Results;
using BuildingBlocks.Domain.Primitives;
using Files.Domain.Errors;
using Files.Domain.ValueObjects;

namespace Files.Domain.Files;

public sealed class File : Entity<FileId>
{
    private File(
        FileId id,
        FileName name,
        ContentType contentType,
        FileSize size,
        DateTimeOffset createdAt,
        Guid? ownerId = null)
        : base(id)
    {
        Name = name;
        ContentType = contentType;
        Size = size;
        Status = FileStatus.Pending;
        CreatedAt = createdAt;
        OwnerId = ownerId;
    }

    private File()
    {
    }

    public FileStatus Status { get; private set; }

    public FileName Name { get; private set; } = default!;

    public ContentType ContentType { get; private set; } = default!;

    public FileSize Size { get; private set; }

    public FileHash? Hash { get; private set; }

    public StorageKey? StorageKey { get; private set; }

    public Guid? OwnerId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public static File Create(
        FileName name,
        ContentType contentType,
        FileSize size,
        DateTimeOffset createdAt,
        Guid? ownerId = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(contentType);

        return new File(
            FileId.New(),
            name,
            contentType,
            size,
            createdAt,
            ownerId);
    }

    public Result SetHash(FileHash hash)
    {
        ArgumentNullException.ThrowIfNull(hash);

        if (Status is FileStatus.Deleted)
        {
            return Result.Failure(
                FileErrors.CannotModifyDeletedFile);
        }

        Hash = hash;

        return Result.Success();
    }

    public Result SetStorageKey(StorageKey storageKey)
    {
        ArgumentNullException.ThrowIfNull(storageKey);

        if (Status is FileStatus.Deleted)
        {
            return Result.Failure(
                FileErrors.CannotModifyDeletedFile);
        }

        StorageKey = storageKey;

        return Result.Success();
    }

    public Result Activate()
    {
        if (Status is FileStatus.Deleted)
        {
            return Result.Failure(
                FileErrors.InvalidStatusTransition);
        }

        if (Status is FileStatus.Active)
        {
            return Result.Success();
        }

        if (Hash is null)
        {
            return Result.Failure(
                FileErrors.HashRequiredForActivation);
        }

        if (StorageKey is null)
        {
            return Result.Failure(
                FileErrors.StorageKeyRequiredForActivation);
        }

        if (Status is not FileStatus.Pending and not FileStatus.Failed)
        {
            return Result.Failure(
                FileErrors.InvalidStatusTransition);
        }

        Status = FileStatus.Active;

        return Result.Success();
    }

    public Result Fail()
    {
        if (Status is FileStatus.Deleted)
        {
            return Result.Failure(
                FileErrors.InvalidStatusTransition);
        }

        if (Status is FileStatus.Active)
        {
            return Result.Failure(
                FileErrors.InvalidStatusTransition);
        }

        Status = FileStatus.Failed;

        return Result.Success();
    }

    public Result Retry()
    {
        if (Status is not FileStatus.Failed)
        {
            return Result.Failure(
                FileErrors.InvalidStatusTransition);
        }

        Status = FileStatus.Pending;

        return Result.Success();
    }

    public Result Delete(DateTimeOffset deletedAt)
    {
        if (Status is FileStatus.Deleted)
        {
            return Result.Failure(
                FileErrors.AlreadyDeleted);
        }

        Status = FileStatus.Deleted;
        DeletedAt = deletedAt;

        return Result.Success();
    }
}