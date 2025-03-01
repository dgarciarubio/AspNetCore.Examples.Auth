namespace AspNetCore.Examples.Auth.Api.Tests;

[Collection(nameof(HostCollectionFixture))]
public class Requests_to_claims_endpoint_should(HostFixture hostFixture)
{
    private readonly TestServer _server = hostFixture.Server;

    [Fact]
    public async Task Succeed_when_authenticated()
    {
        var claims = new[] {
            new Claim { Type = "TestClaim1", Value = "Value1" },
            new Claim { Type = "TestClaim2", Value = "Value2" },
        };

        var response = await _server.CreateRequest("auth/claims")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Claim[]>();
        result.ShouldBe(claims);
    }

    [Fact]
    public async Task Fail_when_not_authenticated()
    {
        var response = await _server.CreateRequest("auth/claims")
            .GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}