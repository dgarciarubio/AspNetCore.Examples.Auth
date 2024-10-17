using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCore.Examples.Auth.Api.Authorization.Endpoints;

internal static class OnlySameAuthor
{
    public static IEndpointRouteBuilder MapOnlySameAuthor(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("auth/only-same-author/{authorEmail}", async (IAuthorizationService authorizationService, ClaimsPrincipal user, string authorEmail) =>
        {
            var resource = new Resource(authorEmail);
            var result = await authorizationService.AuthorizeAsync(user, resource, Policies.IsSameAuthor);
            if (!result.Succeeded)
            {
                return Results.Forbid();
            }

            return Results.Ok("You have access to this resource");
        }).RequireAuthorization();
        return endpoints;
    }
}
