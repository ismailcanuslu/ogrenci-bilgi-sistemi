using System.Runtime.CompilerServices;
using System.Text;
using ImportExport.Application.Abstractions;
using ImportExport.Domain;

namespace ImportExport.Infrastructure.Services;

public sealed class CsvImportExportService : IImportExportService
{
    public async IAsyncEnumerable<(T? Item, RowValidationError? Error)> StreamCsvAsync<T>(
        Stream stream,
        Func<string[], int, (T? Item, RowValidationError? Error)> mapper,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        string? headerLine = await reader.ReadLineAsync(ct).ConfigureAwait(false);
        if (headerLine == null) yield break;

        int rowNumber = 1; // Header is row 1, data starts at row 2
        string? line;

        while ((line = await reader.ReadLineAsync(ct).ConfigureAwait(false)) != null)
        {
            rowNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = ParseCsvLine(line);
            var result = mapper(columns, rowNumber);
            yield return result;
        }
    }

    public async Task ExportCsvAsync<T>(
        Stream destinationStream,
        IEnumerable<T> items,
        Func<T, string[]> rowFormatter,
        string[] headers,
        CancellationToken ct = default)
    {
        using var writer = new StreamWriter(destinationStream, Encoding.UTF8, leaveOpen: true);

        // Write header
        var headerLine = string.Join(",", headers.Select(EscapeCsvValue));
        await writer.WriteLineAsync(headerLine.AsMemory(), ct).ConfigureAwait(false);

        // Write rows
        foreach (var item in items)
        {
            var values = rowFormatter(item);
            var line = string.Join(",", values.Select(EscapeCsvValue));
            await writer.WriteLineAsync(line.AsMemory(), ct).ConfigureAwait(false);
        }

        await writer.FlushAsync(ct).ConfigureAwait(false);
    }

    public async Task<ImportReport> ProcessStreamAsync<T>(
        Stream stream,
        Func<string[], int, (T? Item, RowValidationError? Error)> mapper,
        Func<T, Task> onRowProcessed,
        CancellationToken ct = default)
    {
        var errors = new List<RowValidationError>();
        int totalRows = 0;
        int successfulRows = 0;

        await foreach (var (item, error) in StreamCsvAsync(stream, mapper, ct).ConfigureAwait(false))
        {
            totalRows++;
            if (error != null)
            {
                errors.Add(error);
            }
            else if (item != null)
            {
                try
                {
                    await onRowProcessed(item).ConfigureAwait(false);
                    successfulRows++;
                }
                catch (Exception ex)
                {
                    errors.Add(new RowValidationError(totalRows + 1, "Process", ex.Message));
                }
            }
        }

        return ImportReport.Create(totalRows, successfulRows, errors);
    }

    private static string[] ParseCsvLine(string line)
    {
        var values = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++; // skip escaped quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(sb.ToString().Trim());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }

        values.Add(sb.ToString().Trim());
        return values.ToArray();
    }

    private static string EscapeCsvValue(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
