using System.Security.Claims;

namespace Web.Infrastructure;

public static class ClaimsPrincipalExtensions
{
    public static string? GetIdentityId(this ClaimsPrincipal? claimsPrincipal)
    {
        string? identityId = claimsPrincipal?.FindFirstValue("sub")
            ?? claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return identityId;
    }
}
