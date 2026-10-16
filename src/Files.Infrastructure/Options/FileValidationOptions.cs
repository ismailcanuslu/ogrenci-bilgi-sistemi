namespace Files.Infrastructure.Options;

public sealed class FileValidationOptions
{
    public const string SectionName = "Files:Validation";

    public long MaxFileSizeInBytes { get; set; } = 50 * 1024 * 1024; // 50 MB

    public string[] AllowedContentTypes { get; set; } = [];

    public string[] AllowedExtensions { get; set; } = [];
}
