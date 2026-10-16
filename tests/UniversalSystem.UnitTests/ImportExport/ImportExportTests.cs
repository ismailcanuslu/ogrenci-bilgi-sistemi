using System.Text;
using ImportExport.Domain;
using ImportExport.Infrastructure.Services;
using Xunit;

namespace UniversalSystem.UnitTests.ImportExport;

public class ImportExportTests
{
    private record StudentRow(string StudentNumber, string FullName, string Department);

    [Fact]
    public async Task StreamCsvAsync_ShouldStreamAndParseValidRows()
    {
        // Arrange
        var csvContent = "StudentNumber,FullName,Department\n101,Ahmet Yilmaz,Bilgisayar\n102,Ayse Demir,Elektrik\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var service = new CsvImportExportService();

        // Act
        var results = new List<StudentRow>();
        await foreach (var (item, error) in service.StreamCsvAsync<StudentRow>(stream, (cols, row) =>
        {
            if (cols.Length < 3) return (null, new RowValidationError(row, "Columns", "Eksik kolon"));
            return (new StudentRow(cols[0], cols[1], cols[2]), null);
        }))
        {
            if (item != null) results.Add(item);
        }

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal("101", results[0].StudentNumber);
        Assert.Equal("Ahmet Yilmaz", results[0].FullName);
        Assert.Equal("102", results[1].StudentNumber);
    }

    [Fact]
    public async Task StreamCsvAsync_ShouldCaptureRowValidationErrors()
    {
        // Arrange
        var csvContent = "StudentNumber,FullName,Department\n101,Ahmet Yilmaz,Bilgisayar\n,Gecersiz Ogrenci,Fizik\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var service = new CsvImportExportService();

        // Act
        var validItems = new List<StudentRow>();
        var errors = new List<RowValidationError>();

        await foreach (var (item, error) in service.StreamCsvAsync<StudentRow>(stream, (cols, row) =>
        {
            if (string.IsNullOrWhiteSpace(cols[0]))
                return (null, new RowValidationError(row, "StudentNumber", "Ogrenci numarasi bos olamaz", cols[0]));

            return (new StudentRow(cols[0], cols[1], cols[2]), null);
        }))
        {
            if (item != null) validItems.Add(item);
            if (error != null) errors.Add(error);
        }

        // Assert
        Assert.Single(validItems);
        Assert.Single(errors);
        Assert.Equal(3, errors[0].RowNumber);
        Assert.Equal("StudentNumber", errors[0].ColumnName);
    }

    [Fact]
    public async Task ExportCsvAsync_ShouldWriteProperlyFormattedAndEscapedCsv()
    {
        // Arrange
        var service = new CsvImportExportService();
        var data = new List<StudentRow>
        {
            new("101", "Ahmet, Yilmaz", "Bilgisayar"),
            new("102", "Ayse \"Test\"", "Tip")
        };
        var headers = new[] { "StudentNumber", "FullName", "Department" };
        using var memoryStream = new MemoryStream();

        // Act
        await service.ExportCsvAsync(memoryStream, data, s => new[] { s.StudentNumber, s.FullName, s.Department }, headers);
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream, Encoding.UTF8);
        var csvOutput = await reader.ReadToEndAsync();

        // Assert
        Assert.Contains("StudentNumber,FullName,Department", csvOutput);
        Assert.Contains("\"Ahmet, Yilmaz\"", csvOutput); // Escaped due to comma
        Assert.Contains("\"Ayse \"\"Test\"\"\"", csvOutput); // Escaped due to quotes
    }

    [Fact]
    public async Task ProcessStreamAsync_ShouldProduceAccurateReport()
    {
        // Arrange
        var csvContent = "StudentNumber,FullName,Department\n101,Ahmet,Ceng\nINVALID,Mehmet,Ceng\n103,Ali,Eee\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var service = new CsvImportExportService();
        var processed = new List<StudentRow>();

        // Act
        var report = await service.ProcessStreamAsync<StudentRow>(
            stream,
            (cols, row) =>
            {
                if (cols[0] == "INVALID")
                    return (null, new RowValidationError(row, "StudentNumber", "Gecersiz format"));

                return (new StudentRow(cols[0], cols[1], cols[2]), null);
            },
            student =>
            {
                processed.Add(student);
                return Task.CompletedTask;
            });

        // Assert
        Assert.Equal(3, report.TotalRows);
        Assert.Equal(2, report.SuccessfulRows);
        Assert.Equal(1, report.FailedRows);
        Assert.False(report.IsSuccess);
        Assert.Equal(2, processed.Count);
    }

    [Fact]
    public void ImportJob_Lifecycle_TransitionsStateAccurately()
    {
        // Arrange
        var job = ImportJob.Create("students_2026.csv", "CSV", DateTime.UtcNow);

        // Assert initial
        Assert.Equal(ImportStatus.Pending, job.Status);

        // Act processing
        job.MarkAsProcessing();
        Assert.Equal(ImportStatus.Processing, job.Status);

        // Act completed with partial errors
        job.MarkAsCompleted(100, 95, 5, DateTime.UtcNow, "5 satır hatalı");
        Assert.Equal(ImportStatus.PartiallyCompleted, job.Status);
        Assert.Equal(100, job.TotalRows);
        Assert.Equal(95, job.SuccessfulRows);
        Assert.Equal(5, job.FailedRows);
    }
}
