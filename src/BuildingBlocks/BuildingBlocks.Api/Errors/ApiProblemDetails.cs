using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Api.Errors;

public static class ApiProblemDetails
{
    public static ProblemDetails Create(
        int statusCode,
        string code,
        string title,
        string? detail = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://api.example.com/problems/{code}",
        };

        problemDetails.Extensions["code"] = code;

        return problemDetails;
    }
}