# AspNetCore.Examples.Auth

This project serves as an example of how to authenticate and authorize users in an ASP.Net Core application.

It makes use of the following technologies and projects:

- [.NET 7.0](https://dotnet.microsoft.com/es-es/download/dotnet/7.0)
- [ASP.NET Core 7.0](https://learn.microsoft.com/es-es/aspnet/core/?view=aspnetcore-7.0)
- [Swashbuckle](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-7.0&tabs=visual-studio)
- [OpenID Connect](https://openid.net/developers/how-connect-works/)
- [Duende Identity Server Demo](https://demo.duendesoftware.com/)

# How to run
Simply run the Web Api project from visual studio, or from a terminal by executing `dotnet run --project .\AspNetCore.Examples.Auth.Api\AspNetCore.Examples.Auth.Api.csproj` in the root folder.

Access the web api at http://localhost:5297/swagger and try to call the endpoints:

- Access `/auth/anonymous` to call an endpoint that requires no authentication.
- The rest of the endpoints should return `401 Unauthorized`.
- Click the `Authorize` button to authenticate with Duende Identity Server. Use any of the suggested users in the login page (`alice/alice` or `bob/bob` at the time of writing).
    - Call the `/auth/claims` endpoint to check your claims.
    - Call the `/auth/only-admin` endpoint to use [Role Based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-7.0).
    - Call the `/auth/only-alice` or `/auth/only-bob` endpoints to use [Claims Based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/claims?view=aspnetcore-7.0).
    - Call the `/auth/only-recent-login` endpoint to use [Policy Based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-7.0).
    - Call the `/auth/only-same-author` endpoint with the authenticated user email (check in the `/auth/claims` endpoint) to use [Resource Based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-7.0).
    - Any of these calls should return `403 Forbidden` if the requirements are not met.
