using BuildingBlocks.Domain.Primitives;

namespace ImportExport.Domain;

public sealed class ImportJob : AggregateRoot<ImportJobId>
{
    public string FileName { get; private set; } = string.Empty;
    public string Format { get; private set; } = string.Empty; // CSV, Excel
    public ImportStatus Status { get; private set; }
    public int TotalRows { get; private set; }
    public int SuccessfulRows { get; private set; }
    public int FailedRows { get; private set; }
    public string? ErrorSummary { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    private ImportJob() : base(ImportJobId.New()) { }

    private ImportJob(
        ImportJobId id,
        string fileName,
        string format,
        DateTime createdAtUtc) : base(id)
    {
        FileName = fileName;
        Format = format;
        Status = ImportStatus.Pending;
        CreatedAtUtc = createdAtUtc;
    }

    public static ImportJob Create(string fileName, string format, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Dosya adı boş olamaz.", nameof(fileName));

        return new ImportJob(ImportJobId.New(), fileName, format, nowUtc);
    }

    public void MarkAsProcessing()
    {
        Status = ImportStatus.Processing;
    }

    public void MarkAsCompleted(int totalRows, int successfulRows, int failedRows, DateTime completedAtUtc, string? errorSummary = null)
    {
        TotalRows = totalRows;
        SuccessfulRows = successfulRows;
        FailedRows = failedRows;
        CompletedAtUtc = completedAtUtc;
        ErrorSummary = errorSummary;

        if (failedRows == 0)
        {
            Status = ImportStatus.Completed;
        }
        else if (successfulRows > 0)
        {
            Status = ImportStatus.PartiallyCompleted;
        }
        else
        {
            Status = ImportStatus.Failed;
        }
    }

    public void MarkAsFailed(string error, DateTime failedAtUtc)
    {
        Status = ImportStatus.Failed;
        ErrorSummary = error;
        CompletedAtUtc = failedAtUtc;
    }
}
