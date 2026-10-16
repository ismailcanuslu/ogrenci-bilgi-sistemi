using BuildingBlocks.Application.Results;
using Files.Domain.Errors;

namespace Files.Domain.ValueObjects;

public readonly record struct FileSize
{
    public long Bytes { get; }

    private FileSize(long bytes)
    {
        Bytes = bytes;
    }

    public static Result<FileSize> Create(long bytes)
    {
        if (bytes < 0)
        {
            return Result<FileSize>.Failure(
                FileErrors.InvalidSize);
        }

        return Result<FileSize>.Success(
            new FileSize(bytes));
    }

    public bool IsEmpty => Bytes == 0;

    public double Kilobytes => Bytes / 1024d;

    public double Megabytes => Bytes / (1024d * 1024d);

    public double Gigabytes => Bytes / (1024d * 1024d * 1024d);

    public override string ToString()
        => $"{Bytes} bytes";
}