namespace BuildingBlocks.Api.Options;

public sealed class ProblemDetailsOptions
{
    public const string SectionName = "ProblemDetails";

    public string TypeBaseUrl { get; set; } =
        "https://api.example.com/problems";
}