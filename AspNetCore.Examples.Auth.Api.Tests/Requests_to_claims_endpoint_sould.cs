

namespace AspNetCore.Examples.Auth.Api.Tests;

[Collection(nameof(HostCollectionFixture))]
public class Requests_to_claims_endpoint_sould(HostFixture hostFixture)
{
    private readonly TestServer _server = hostFixture.Server;

    [Fact]
    public async Task Succceed_when_authenticated()
    {
        var claims = new[] {
            new Claim { Type = "TestClaim1", Value = "Value1" },
            new Claim { Type = "TestClaim2", Value = "Value2" },
        };

        var response = await _server.CreateRequest("auth/claims")
            .WithAuthorizationHeader(claims)
            .GetAsync();

        response.Should().BeSuccessful();
        var result = await response.Content.ReadFromJsonAsync<Claim[]>();
        result.Should().BeEquivalentTo(claims);
    }

    [Fact]
    public async Task Fail_when_not_authenticated()
    {
        var response = await _server.CreateRequest("auth/claims")
            .GetAsync();

        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}