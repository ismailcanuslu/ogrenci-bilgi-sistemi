namespace Files.Infrastructure.Options;

public sealed class FileSecurityOptions
{
    public const string SectionName = "Files:Security";

    public bool EnforceAuthorization { get; set; } = false;

    public bool AllowAnonymousRead { get; set; } = false;

    public string AdminRoleName { get; set; } = "Admin";
}
