using System.Text.Json;

namespace Microsoft.AspNetCore.TestHost;

public static class RequestBuilderExtensions
{
    public static RequestBuilder WithAuthorizationHeader(this RequestBuilder builder, params global::AspNetCore.Examples.Auth.Api.Claim[] claims)
    {
        return builder
            .AddHeader("Authorization", JsonSerializer.Serialize(claims));
    }
}
