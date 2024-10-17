using AspNetCore.Examples.Auth.Api.Authorization.Endpoints;

namespace AspNetCore.Examples.Auth.Api.Authorization;

internal static class Extensions
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapAnonymous()
            .MapClaims()
            .MapOnlyAdmin()
            .MapOnlyAlice()
            .MapOnlyBob()
            .MapOnlyRecentLogin()
            .MapOnlySameAuthor();
    }
}
