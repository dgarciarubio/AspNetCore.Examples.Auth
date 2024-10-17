namespace AspNetCore.Examples.Auth.Api.Authorization.Endpoints;

internal static class OnlyRecentLogin
{
    public static IEndpointRouteBuilder MapOnlyRecentLogin(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("auth/only-recent-login", () =>
        {
            return "Hello recently logged user!";
        }).RequireAuthorization(Policies.IsRecentLogin);
        return endpoints;
    }
}
