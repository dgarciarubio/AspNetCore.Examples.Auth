namespace AspNetCore.Examples.Auth.Api.Tests;

[Collection(nameof(HostCollectionFixture))]
public class Requests_to_only_admin_endpoint_should(HostFixture hostFixture)
{
    private readonly TestServer _server = hostFixture.Server;

    [Fact]
    public async Task Succeed_when_user_is_admin()
    {
        var claims = new[] { new Claim
        {
            Type = ClaimTypes.Role,
            Value = Authorization.Roles.Admin
        }};

        var response = await _server.CreateRequest("auth/only-admin")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Fail_when_user_is_not_admin()
    {
        var claims = new[] { new Claim
        {
            Type = ClaimTypes.Role,
            Value = "OtherRole",
        }};

        var response = await _server.CreateRequest("auth/only-admin")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Fail_when_not_authenticated()
    {
        var response = await _server.CreateRequest("auth/only-admin")
            .GetAsync();

       response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}