namespace ImportExport.Domain;

public sealed record RowValidationError(
    int RowNumber,
    string ColumnName,
    string ErrorMessage,
    string? RawValue = null);

public sealed class ImportReport
{
    public int TotalRows { get; init; }
    public int ProcessedRows { get; init; }
    public int SuccessfulRows { get; init; }
    public int FailedRows => ValidationErrors.Count;
    public bool IsSuccess => FailedRows == 0 && ProcessedRows > 0;
    public List<RowValidationError> ValidationErrors { get; init; } = new();

    public static ImportReport Create(int totalRows, int successfulRows, List<RowValidationError> errors)
    {
        return new ImportReport
        {
            TotalRows = totalRows,
            ProcessedRows = successfulRows + errors.Count,
            SuccessfulRows = successfulRows,
            ValidationErrors = errors
        };
    }
}
