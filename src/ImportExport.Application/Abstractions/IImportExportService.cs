using ImportExport.Domain;

namespace ImportExport.Application.Abstractions;

public interface IImportExportService
{
    /// <summary>
    /// CSV akışını satır satır belleği tüketmeden stream eder ve her satırı doğrular/map eder.
    /// </summary>
    IAsyncEnumerable<(T? Item, RowValidationError? Error)> StreamCsvAsync<T>(
        Stream stream,
        Func<string[], int, (T? Item, RowValidationError? Error)> mapper,
        CancellationToken ct = default);

    /// <summary>
    /// Verilen nesne koleksiyonunu CSV başlıkları ve satır biçimlendiricisi ile hedef Stream'e yazar.
    /// </summary>
    Task ExportCsvAsync<T>(
        Stream destinationStream,
        IEnumerable<T> items,
        Func<T, string[]> rowFormatter,
        string[] headers,
        CancellationToken ct = default);

    /// <summary>
    /// Bir CSV akışını işler, başarılı kayıtları işlem fonksiyonuna aktarır ve detaylı ImportReport döner.
    /// </summary>
    Task<ImportReport> ProcessStreamAsync<T>(
        Stream stream,
        Func<string[], int, (T? Item, RowValidationError? Error)> mapper,
        Func<T, Task> onRowProcessed,
        CancellationToken ct = default);
}
