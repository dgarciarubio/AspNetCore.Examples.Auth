namespace AspNetCore.Examples.Auth.Api.Tests;

[Collection(nameof(HostCollectionFixture))]
public class Requests_to_onlysameauthor_endpoint_sould(HostFixture hostFixture)
{
    private readonly TestServer _server = hostFixture.Server;

    [Fact]
    public async Task Succeed_when_requested_resource_is_from_the_same_author()
    {
        var authorEmail = "author@email.com";
        var claims = new[] { new Claim
        {
            Type = ClaimTypes.Email,
            Value = authorEmail,
        }};

        var response = await _server.CreateRequest($"auth/only-same-author/{authorEmail}")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Fail_when_requested_resource_is_not_from_the_same_author()
    {
        var claims = new[] { new Claim
        {
            Type = ClaimTypes.Email,
            Value = "other@email.com",
        }};

        var response = await _server.CreateRequest("auth/only-same-author/author@email.com")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Fail_when_not_authenticated()
    {
        var response = await _server.CreateRequest("auth/only-same-author/author@email.com")
            .GetAsync();

        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}