namespace Files.Infrastructure.Options;

public sealed class S3StorageOptions
{
    public const string SectionName = "Files:Storage:S3";

    public string? ServiceUrl { get; set; } = "http://localhost:9000";

    public string BucketName { get; set; } = "obs-files";

    public string AccessKey { get; set; } = "minioadmin";

    public string SecretKey { get; set; } = "minioadmin";

    public string Region { get; set; } = "eu-central-1";

    public bool ForcePathStyle { get; set; } = true;
}
