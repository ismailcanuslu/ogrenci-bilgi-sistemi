using BuildingBlocks.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Api.Exceptions;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            return await WriteBadRequestAsync(
                httpContext,
                "Validation.Failed",
                validationException.Errors
                    .Select(e => new
                    {
                        code = e.ErrorCode,
                        message = e.ErrorMessage,
                        type = "Validation",
                        path = e.PropertyName
                    })
                    .ToArray(),
                cancellationToken);
        }

        if (exception is DomainException domainException)
        {
            return await WriteBadRequestAsync(
                httpContext,
                domainException.Code,
                new[] { new { code = domainException.Code, message = domainException.Code, type = "Validation", path = (string?)null } },
                cancellationToken);
        }

        logger.LogError(
            exception,
            "Unhandled exception occurred. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "about:blank",
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }

    private static async ValueTask<bool> WriteBadRequestAsync(
        HttpContext httpContext,
        string code,
        object errors,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = code,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["code"] = code;
        problemDetails.Extensions["errors"] = errors;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
