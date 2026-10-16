namespace Files.Infrastructure.Options;

public sealed class FileStorageOptions
{
    public const string SectionName = "Files:Storage";

    /// <summary>
    /// Storage provider: "S3" (Default, supports MinIO & AWS S3) or "Physical"
    /// </summary>
    public string Provider { get; set; } = "S3";

    public string BasePath { get; set; } = "uploads";

    public S3StorageOptions S3 { get; set; } = new();
}
