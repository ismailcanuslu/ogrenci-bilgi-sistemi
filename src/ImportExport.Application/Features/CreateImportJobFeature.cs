using BuildingBlocks.Application.Abstractions.Time;
using BuildingBlocks.Application.Results;
using ImportExport.Application.Abstractions;
using ImportExport.Domain;

namespace ImportExport.Application.Features;

public sealed record CreateImportJobRequest(
    string FileName,
    string Format);

public sealed record ImportJobDto(
    Guid Id,
    string FileName,
    string Format,
    string Status,
    int TotalRows,
    int SuccessfulRows,
    int FailedRows,
    string? ErrorSummary,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc);

public static class CreateImportJobFeature
{
    public static async Task<Result<ImportJobDto>> HandleAsync(
        CreateImportJobRequest request,
        IImportJobRepository repository,
        IDateTimeProvider dateTimeProvider,
        CancellationToken ct = default)
    {
        try
        {
            var job = ImportJob.Create(request.FileName, request.Format, dateTimeProvider.UtcNow.UtcDateTime);
            await repository.AddAsync(job, ct).ConfigureAwait(false);

            return Result<ImportJobDto>.Success(new ImportJobDto(
                job.Id.Value,
                job.FileName,
                job.Format,
                job.Status.ToString(),
                job.TotalRows,
                job.SuccessfulRows,
                job.FailedRows,
                job.ErrorSummary,
                job.CreatedAtUtc,
                job.CompletedAtUtc));
        }
        catch (ArgumentException)
        {
            return Result<ImportJobDto>.Failure(ImportErrors.UnsupportedFormat);
        }
    }
}
