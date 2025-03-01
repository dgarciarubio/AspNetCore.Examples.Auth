namespace AspNetCore.Examples.Auth.Api.Tests;

[Collection(nameof(HostCollectionFixture))]
public class Requests_to_only_alice_endpoint_should(HostFixture hostFixture)
{
    private readonly TestServer _server = hostFixture.Server;

    [Fact]
    public async Task Succeed_when_user_is_alice()
    {
        var claims = new[] { new Claim
        {
            Type = ClaimTypes.Email,
            Value = "AliceSmith@email.com",
        }};

        var response = await _server.CreateRequest("auth/only-alice")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Fail_when_user_is_not_alice()
    {
        var claims = new[] { new Claim
        {
            Type = ClaimTypes.Email,
            Value = "Other@email.com",
        }};

        var response = await _server.CreateRequest("auth/only-alice")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Fail_when_not_authenticated()
    {
        var response = await _server.CreateRequest("auth/only-alice")
            .GetAsync();

       response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}