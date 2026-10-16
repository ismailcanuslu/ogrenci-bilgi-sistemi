using BuildingBlocks.Application.Results;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Errors;

public static class ServiceResultHttpMapper
{
    public static IResult ToHttpResult<T>(ServiceResult<T> result, string? culture = null)
        => result.ToHttpResult(culture);

    public static IResult ToHttpResult(ServiceResult result, string? culture = null)
        => result.ToHttpResult(culture);

    public static IResult ToHttpResult<T>(ServiceResult<T> result, HttpContext httpContext)
    {
        var lang = httpContext.Request.Query["lang"].ToString();
        if (string.IsNullOrWhiteSpace(lang))
        {
            lang = httpContext.Request.Headers.AcceptLanguage.ToString().Split(',')[0].Trim();
        }
        return result.ToHttpResult(lang);
    }

    public static IResult ToHttpResult(ServiceResult result, HttpContext httpContext)
    {
        var lang = httpContext.Request.Query["lang"].ToString();
        if (string.IsNullOrWhiteSpace(lang))
        {
            lang = httpContext.Request.Headers.AcceptLanguage.ToString().Split(',')[0].Trim();
        }
        return result.ToHttpResult(lang);
    }
}