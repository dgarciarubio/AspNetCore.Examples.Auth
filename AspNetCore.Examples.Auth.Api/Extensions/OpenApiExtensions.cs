using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Runtime.CompilerServices;

namespace AspNetCore.Examples.Auth.Api.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddCustomOpenApi(this IServiceCollection services)
    {
        return services
            .AddOpenApi(options => options
                .AddDocumentTransformer<OpenApiAuthDocumentTransformer>()
                .AddOperationTransformer<OpenApiAuthOperationTransformer>()
            );
    }

    public static WebApplication UseCustomOpenApi(this WebApplication app, IConfiguration configuration)
    {
        app.MapOpenApi();
        app.UseSwaggerUI(configuration.GetSection("OpenApi:SwaggerUI").Bind);
        app.MapScalarApiReference(configuration.GetSection("OpenApi:Scalar").Bind);

        return app;
    }

    private static bool IsAuthorizedOperation(this ActionDescriptor actionDescriptor)
    {
        bool authorize = actionDescriptor.EndpointMetadata.Any(m => m is AuthorizeAttribute);
        bool allowAnonymous = actionDescriptor.EndpointMetadata.Any(m => m is AllowAnonymousAttribute);

        return authorize && !allowAnonymous;
    }

    private class OpenApiAuthDocumentTransformer(IConfiguration configuration, IHttpClientFactory httpClientFactory) : IOpenApiDocumentTransformer
    {
        private readonly HttpClient _discoveryDocumentClient = httpClientFactory.CreateClient("DiscoveryDocument");
        private readonly IConfigurationSection _securitySchemesConfig = configuration.GetSection("OpenApi:SecuritySchemes");

        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            await foreach (var securityScheme in GetSecuritySchemes(context.ApplicationServices, cancellationToken))
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes.Add(securityScheme.Key, securityScheme.Value);
            }
        }

        private async IAsyncEnumerable<KeyValuePair<string, OpenApiSecurityScheme>> GetSecuritySchemes(IServiceProvider serviceProvider, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var schemeConfig in _securitySchemesConfig.GetChildren())
            {
                var schemeKey = schemeConfig.Key;
                var scheme = schemeConfig.Get<OpenApiSecurityScheme>();
                if (scheme is not null)
                {
                    await Configure(scheme.Flows, schemeConfig.GetSection(nameof(scheme.Flows)), cancellationToken);
                    yield return new(schemeKey, scheme);
                }
            }
        }

        private async Task Configure(OpenApiOAuthFlows flows, IConfigurationSection flowsConfig, CancellationToken cancellationToken)
        {
            var flowsEnum = new KeyValuePair<string, OpenApiOAuthFlow>[] {
                new (nameof(flows.AuthorizationCode), flows.AuthorizationCode),
                new (nameof(flows.ClientCredentials), flows.ClientCredentials),
                new (nameof(flows.Password), flows.Password),
                new (nameof(flows.Implicit), flows.Implicit),
            }.Where(f => f.Value is not null);
            foreach (var flow in flowsEnum)
            {
                var flowConfig = flowsConfig.GetSection(flow.Key);
                Configure(flow.Value.Scopes, flowConfig.GetSection(nameof(flow.Value.Scopes)));
                await DiscoverUrls(flow.Value, flowConfig, cancellationToken);
            }
        }

        private void Configure(IDictionary<string, string> scopes, IConfigurationSection scopesConfig)
        {
            foreach (var scopeConfig in scopesConfig.GetChildren())
            {
                var value = scopeConfig.GetValue<string>("Value");
                var description = scopeConfig.GetValue<string>("Description");
                if (value is not null)
                {
                    scopes.Add(value, description ?? value);
                }
            }
        }

        private async Task DiscoverUrls(OpenApiOAuthFlow flow, IConfigurationSection flowConfig, CancellationToken cancellationToken)
        {
            if (flow.TokenUrl is null || flow.AuthorizationUrl is null)
            {
                var request = flowConfig.GetSection("DiscoveryDocumentRequest").Get<DiscoveryDocumentRequest>();
                if (request is not null)
                {
                    var response = await _discoveryDocumentClient.GetDiscoveryDocumentAsync(request, cancellationToken);
                    flow.TokenUrl = flow.TokenUrl ?? (response.TokenEndpoint is not null ? new Uri(response.TokenEndpoint) : null);
                    flow.AuthorizationUrl = flow.AuthorizationUrl ?? (response.AuthorizeEndpoint is not null ? new Uri(response.AuthorizeEndpoint) : null);
                }
            }
        }
    }

    private class OpenApiAuthOperationTransformer(IConfiguration configuration) : IOpenApiOperationTransformer
    {
        private static readonly OpenApiResponses _authorizedResponses = new()
        {
            ["401"] = new OpenApiResponse { Description = "User not authenticated." },
            ["403"] = new OpenApiResponse { Description = "User not authorized to perform this action." },
        };

        private readonly IEnumerable<OpenApiSecurityRequirement> _securityRequirements = GetSecurityRequirements(configuration.GetSection("OpenApi:SecurityRequirements"))
            .ToArray();

        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (!context.Description.ActionDescriptor.IsAuthorizedOperation())
                return Task.CompletedTask;

            foreach (var response in _authorizedResponses.Where(r => !operation.Responses.ContainsKey(r.Key)))
            {
                operation.Responses.Add(response.Key, response.Value);
            }

            foreach (var securityRequirement in _securityRequirements)
            {
                operation.Security.Add(securityRequirement);
            }

            return Task.CompletedTask;
        }

        private static IEnumerable<OpenApiSecurityRequirement> GetSecurityRequirements(IConfigurationSection securityRequirementsConfig)
        {
            foreach (var requirementConfig in securityRequirementsConfig.GetChildren())
            {
                var requirement = new OpenApiSecurityRequirement();
                foreach (var schemeConfig in requirementConfig.GetChildren())
                {
                    var scheme = schemeConfig.GetSection("SecurityScheme").Get<OpenApiSecurityScheme>();
                    var scopes = schemeConfig.GetSection("Scopes").Get<string[]>();
                    if (scheme is not null)
                    {
                        requirement.Add(scheme, scopes ?? []);
                    }
                }
                if (requirement.Any())
                {
                    yield return requirement;
                }
            }
        }
    }
}
