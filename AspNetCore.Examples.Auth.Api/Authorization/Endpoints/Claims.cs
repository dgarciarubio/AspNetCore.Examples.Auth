using System.Security.Claims;

namespace AspNetCore.Examples.Auth.Api.Authorization.Endpoints;

internal static class Claims
{
    public static IEndpointRouteBuilder MapClaims(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("auth/claims", (ClaimsPrincipal user) =>
        {
            return user.Claims.Select(c => new Claim
            {
                Type = c.Type,
                Value = c.Value,
            });
        }).RequireAuthorization();
        return endpoints;
    }
}
