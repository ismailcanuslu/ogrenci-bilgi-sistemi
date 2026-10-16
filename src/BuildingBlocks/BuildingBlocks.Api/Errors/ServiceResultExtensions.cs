using BuildingBlocks.Application.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Translations.Domain;

namespace BuildingBlocks.Api.Errors;

public static class ServiceResultExtensions
{
    public static IResult ToHttpResult<T>(
        this ServiceResult<T> result,
        HttpContext httpContext)
    {
        var culture = httpContext.Request.Query["lang"].FirstOrDefault()
            ?? httpContext.Request.Headers.AcceptLanguage.FirstOrDefault();
        return result.ToHttpResult(culture);
    }

    public static IResult ToHttpResult(
        this ServiceResult result,
        HttpContext httpContext)
    {
        var culture = httpContext.Request.Query["lang"].FirstOrDefault()
            ?? httpContext.Request.Headers.AcceptLanguage.FirstOrDefault();
        return result.ToHttpResult(culture);
    }

    public static IResult ToHttpResult<T>(
        this Result<T> result,
        HttpContext httpContext)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        var firstError = result.Errors.FirstOrDefault();
        var statusCode = firstError?.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var culture = httpContext.Request.Query["lang"].FirstOrDefault()
            ?? httpContext.Request.Headers.AcceptLanguage.FirstOrDefault();

        return CreateProblemDetails(statusCode, result.Errors, culture);
    }

    public static IResult ToHttpResult(
        this Result result,
        HttpContext httpContext)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok();
        }

        var firstError = result.Errors.FirstOrDefault();
        var statusCode = firstError?.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var culture = httpContext.Request.Query["lang"].FirstOrDefault()
            ?? httpContext.Request.Headers.AcceptLanguage.FirstOrDefault();

        return CreateProblemDetails(statusCode, result.Errors, culture);
    }

    public static IResult ToHttpResult<T>(
        this ServiceResult<T> result,
        string? culture = null)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode switch
            {
                StatusCodes.Status200OK =>
                    TypedResults.Ok(result.Data),

                StatusCodes.Status201Created =>
                    TypedResults.Created(string.Empty, result.Data),

                StatusCodes.Status204NoContent =>
                    TypedResults.NoContent(),

                _ =>
                    TypedResults.Json(
                        result.Data,
                        statusCode: result.StatusCode)
            };
        }

        return CreateProblemDetails(result.StatusCode, result.Errors, culture);
    }

    public static IResult ToHttpResult(
        this ServiceResult result,
        string? culture = null)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode switch
            {
                StatusCodes.Status200OK =>
                    TypedResults.Ok(),

                StatusCodes.Status201Created =>
                    TypedResults.Created(),

                StatusCodes.Status204NoContent =>
                    TypedResults.NoContent(),

                _ =>
                    TypedResults.StatusCode(result.StatusCode)
            };
        }

        return CreateProblemDetails(result.StatusCode, result.Errors, culture);
    }

    private static IResult CreateProblemDetails(
        int statusCode,
        IReadOnlyCollection<Error> errors,
        string? culture = null)
    {
        var firstError = errors.FirstOrDefault();
        var targetCulture = LanguageCode.Normalize(culture);
        var detailText = firstError is not null
            ? TranslationCatalog.Translate(firstError.Code, targetCulture)
            : GetTitle(statusCode);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = detailText,
        };

        if (firstError is not null)
        {
            problemDetails.Extensions["code"] = firstError.Code;
        }

        if (errors.Count > 0)
        {
            problemDetails.Extensions["errors"] = errors
                .Select(error => new
                {
                    code = error.Code,
                    message = TranslationCatalog.Translate(error.Code, targetCulture),
                    type = error.Type.ToString(),
                    path = error.Path
                })
                .ToArray();
        }

        return TypedResults.Problem(problemDetails);
    }

    private static string GetTitle(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status409Conflict => "Conflict",
            StatusCodes.Status500InternalServerError => "Internal Server Error",
            _ => "An error occurred"
        };
    }
}