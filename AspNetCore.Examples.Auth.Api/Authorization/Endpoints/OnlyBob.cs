namespace AspNetCore.Examples.Auth.Api.Authorization.Endpoints;

internal static class OnlyBob
{
    public static IEndpointRouteBuilder MapOnlyBob(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("auth/only-bob", () =>
        {
            return "Hello Bob!";
        }).RequireAuthorization(Policies.IsBob);
        return endpoints;
    }
}
