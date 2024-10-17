namespace AspNetCore.Examples.Auth.Api.Tests;

[Collection(nameof(HostCollectionFixture))]
public class Requests_to_only_bob_endpoint_should(HostFixture hostFixture)
{
    private readonly TestServer _server = hostFixture.Server;

    [Fact]
    public async Task Succeed_when_user_is_bob()
    {
        var claims = new[] { new Claim
        {
            Type = ClaimTypes.Email,
            Value = "BobSmith@email.com",
        }};

        var response = await _server.CreateRequest("auth/only-bob")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Fail_when_user_is_not_bob()
    {
        var claims = new[] { new Claim
        {
            Type = ClaimTypes.Email,
            Value = "Other@email.com",
        }};

        var response = await _server.CreateRequest("auth/only-bob")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Fail_when_not_authenticated()
    {
        var response = await _server.CreateRequest("auth/only-bob")
            .GetAsync();

        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}