namespace AspNetCore.Examples.Auth.Api.Tests;

[Collection(nameof(HostCollectionFixture))]
public class Requests_to_only_recent_login_endpoint_should(HostFixture hostFixture)
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

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
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

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Fail_when_the_login_time_is_not_known()
    {
        var response = await _server.CreateRequest("auth/only-recent-login")
            .WithAuthorizationHeader()
            .GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Fail_when_not_authenticated()
    {
        var response = await _server.CreateRequest("auth/only-recent-login")
            .GetAsync();

       response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}