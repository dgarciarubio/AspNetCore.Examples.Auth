namespace AspNetCore.Examples.Auth.Api.Authorization.Endpoints;

internal static class OnlyAlice
{
    public static IEndpointRouteBuilder MapOnlyAlice(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("auth/only-alice", () =>
        {
            return "Hello Alice!";
        }).RequireAuthorization(Policies.IsAlice);
        return endpoints;
    }
}
