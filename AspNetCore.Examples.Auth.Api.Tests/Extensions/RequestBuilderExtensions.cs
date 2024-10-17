using System.Text.Json;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.TestHost;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class RequestBuilderExtensions
{
    public static RequestBuilder WithAuthorizationHeader(this RequestBuilder builder, params global::AspNetCore.Examples.Auth.Api.Claim[] claims)
    {
        return builder
            .AddHeader("Authorization", JsonSerializer.Serialize(claims));
    }
}
