namespace AspNetCore.Examples.Auth.Api.Tests;

[Collection(nameof(HostCollectionFixture))]
public class Requests_to_onlyrecentlogin_endpoint_sould(HostFixture hostFixture)
{
    private readonly TestServer _server = hostFixture.Server;

    [Fact]
    public async Task Succeed_when_the_login_is_recent()
    {
        var unixAuthTime = DateTime.UtcNow - DateTime.UnixEpoch;
        var claims = new[] { new Claim
        {
            Type = "auth_time",
            Value = unixAuthTime.TotalSeconds.ToString(),
        }};

        var response = await _server.CreateRequest("auth/only-recent-login")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Fail_when_the_login_is_not_recent()
    {
        var unixAuthTime = DateTime.UtcNow - TimeSpan.FromDays(100) - DateTime.UnixEpoch;
        var claims = new[] { new Claim
        {
            Type = "auth_time",
            Value = unixAuthTime.TotalSeconds.ToString(),
        }};

        var response = await _server.CreateRequest("auth/only-recent-login")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Fail_when_the_login_time_is_not_known()
    {
        var response = await _server.CreateRequest("auth/only-recent-login")
            .WithAuthorizationHeader()
            .GetAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Fail_when_not_authenticated()
    {
        var response = await _server.CreateRequest("auth/only-recent-login")
            .GetAsync();

        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}