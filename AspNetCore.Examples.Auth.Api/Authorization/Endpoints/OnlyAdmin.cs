using Microsoft.AspNetCore.Authorization;

namespace AspNetCore.Examples.Auth.Api.Authorization.Endpoints;

internal static class OnlyAdmin
{
    public static IEndpointRouteBuilder MapOnlyAdmin(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("auth/only-admin", [Authorize(Roles = Roles.Admin)] () =>
        {
            return "Hello admin user!";
        });
        return endpoints;
    }
}
