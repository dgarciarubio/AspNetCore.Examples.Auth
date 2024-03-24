# AspNetCore.Examples.Auth

This project serves as an example of how to authenticate and authorize users in an ASP.Net Core application.

It makes use of the following technologies and projects:

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [ASP.NET Core 8.0](https://learn.microsoft.com/aspnet/core/?view=aspnetcore-8.0)
- [Swashbuckle](https://learn.microsoft.com/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0)
- [OpenID Connect](https://openid.net/developers/how-connect-works/)
- [Duende Identity Server Demo](https://demo.duendesoftware.com/)
- [XUnit](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Coverlet](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator](https://reportgenerator.io/)

# How to run
Simply run the Web Api project from Visual Studio, or from a terminal by executing `dotnet run --project .\AspNetCore.Examples.Auth.Api\AspNetCore.Examples.Auth.Api.csproj` in the root folder.

Access the web api at http://localhost:5297/swagger and try to call the endpoints:

- Access `/auth/anonymous` to call an endpoint that requires no authentication.
- The rest of the endpoints should return `401 Unauthorized`.
- Click the `Authorize` button to authenticate with Duende Identity Server. Use any of the suggested users in the login page (`alice/alice` or `bob/bob` at the time of writing).
    - Call the `/auth/claims` endpoint to check your claims.
    - Call the `/auth/only-admin` endpoint to use [Role Based Authorization](https://learn.microsoft.com/aspnet/core/security/authorization/roles?view=aspnetcore-8.0).
    - Call the `/auth/only-alice` or `/auth/only-bob` endpoints to use [Claims Based Authorization](https://learn.microsoft.com/aspnet/core/security/authorization/claims?view=aspnetcore-8.0).
    - Call the `/auth/only-recent-login` endpoint to use [Policy Based Authorization](https://learn.microsoft.com/aspnet/core/security/authorization/policies?view=aspnetcore-8.0).
    - Call the `/auth/only-same-author` endpoint with the authenticated user email (check in the `/auth/claims` endpoint) to use [Resource Based Authorization](https://learn.microsoft.com/aspnet/core/security/authorization/resourcebased?view=aspnetcore-8.0).
    - Any of these calls should return `403 Forbidden` if the requirements are not met.

# How to test
Simply run the tests from the Test Explorer in Visual Studio, or from a terminal by executing `dotnet test` in the root folder.
[More about ASP.NET Core 8.0 integration tests](https://learn.microsoft.com/aspnet/core/test/integration-tests?view=aspnetcore-8.0)

To run tests and generate an html coverage analysis report, run the `coverage_report.ps1` powershell script  in the root folder.
[More about code coverage for unit testing in .NET](https://learn.microsoft.com/dotnet/core/testing/unit-testing-code-coverage)