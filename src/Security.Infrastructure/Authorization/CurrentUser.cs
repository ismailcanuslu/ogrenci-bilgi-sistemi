using BuildingBlocks.Application.Abstractions.Security;
using Microsoft.AspNetCore.Http;

namespace Security.Infrastructure.Authorization;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var principal = httpContextAccessor.HttpContext?.User;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var value = principal.FindFirst("sub")?.Value
                        ?? principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(value, out var id) && id != Guid.Empty ? id : null;
        }
    }
}
