namespace AspNetCore.Examples.Auth.Api.Tests;

[Collection(nameof(HostCollectionFixture))]
public class Requests_to_anonymous_endpoint_should(HostFixture hostFixture)
{
    private readonly TestServer _server = hostFixture.Server;

    [Fact]
    public async Task Succeed_always()
    {
        var response = await _server.CreateRequest("auth/anonymous")
            .GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}