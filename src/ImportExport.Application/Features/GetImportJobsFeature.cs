using BuildingBlocks.Application.Results;
using ImportExport.Application.Abstractions;

namespace ImportExport.Application.Features;

public static class GetImportJobsFeature
{
    public static async Task<Result<IReadOnlyList<ImportJobDto>>> HandleAsync(
        IImportJobRepository repository,
        CancellationToken ct = default)
    {
        var jobs = await repository.GetAllAsync(ct).ConfigureAwait(false);

        var dtos = jobs.Select(j => new ImportJobDto(
            j.Id.Value,
            j.FileName,
            j.Format,
            j.Status.ToString(),
            j.TotalRows,
            j.SuccessfulRows,
            j.FailedRows,
            j.ErrorSummary,
            j.CreatedAtUtc,
            j.CompletedAtUtc)).ToList();

        return Result<IReadOnlyList<ImportJobDto>>.Success(dtos);
    }
}
