namespace AspNetCore.Examples.Auth.Api.Authorization.Endpoints;

internal static class Anonymous
{
    public static IEndpointRouteBuilder MapAnonymous(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("auth/anonymous", () =>
        {
            return "Hello kind stranger!";
        });
        return endpoints;
    }
}
