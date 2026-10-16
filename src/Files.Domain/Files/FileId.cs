namespace Files.Domain.Files;

public readonly record struct FileId(Guid Value)
{
    public static FileId New() => new(Guid.NewGuid());
}