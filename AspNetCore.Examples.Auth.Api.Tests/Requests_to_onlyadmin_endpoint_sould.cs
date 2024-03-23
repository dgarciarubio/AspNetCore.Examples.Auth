namespace AspNetCore.Examples.Auth.Api.Tests;

[Collection(nameof(HostCollectionFixture))]
public class Requests_to_onlyadmin_endpoint_sould(HostFixture hostFixture)
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

        response.Should().BeSuccessful();
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

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Fail_when_not_authenticated()
    {
        var response = await _server.CreateRequest("auth/only-admin")
            .GetAsync();

        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}