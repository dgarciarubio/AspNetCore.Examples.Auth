using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace AspNetCore.Examples.Auth.Api.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddCustomOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSingleton<DiscoveryDocumentProvider>()
            .AddOpenApi(options =>
            {
                options.AddDocumentTransformer(new OpenApiAuthDocumentTransformer());
                options.AddOperationTransformer(new OpenApiAuthOperationTransformer());
            });
    }

    public static WebApplication UseCustomOpenApi(this WebApplication app, IConfiguration configuration)
    {
        app.MapOpenApi();

        var swaggerUIConfig = configuration.GetSection("OpenApi:SwaggerUI");
        if (swaggerUIConfig.Exists())
        {
            app.UseSwaggerUI(config =>
            {
                config.ConfigObject.Urls = [new() { Name = "V1", Url = "/openapi/v1.json" }];
                swaggerUIConfig.Bind(config);
            });
        }

        var scalarConfig = configuration.GetSection("OpenApi:Scalar");
        if (scalarConfig.Exists())
        {
            app.MapScalarApiReference(scalarConfig.Bind);
        }

        return app;
    }

    private static bool IsAuthorizedOperation(this ActionDescriptor actionDescriptor)
    {
        bool authorize = actionDescriptor.EndpointMetadata.Any(m => m is AuthorizeAttribute);
        bool allowAnonymous = actionDescriptor.EndpointMetadata.Any(m => m is AllowAnonymousAttribute);

        return authorize && !allowAnonymous;
    }

    private class DiscoveryDocumentProvider
    {
        private readonly IOptionsMonitor<JwtBearerOptions> _jwtBearerOptions;
        private readonly HttpClient _httpClient;
        private DiscoveryDocumentResponse? _value;

        public DiscoveryDocumentProvider(IHttpClientFactory httpClientFactory, IOptionsMonitor<JwtBearerOptions> jwtBearerOptions)
        {
            _jwtBearerOptions = jwtBearerOptions;
            _httpClient = httpClientFactory.CreateClient(nameof(DiscoveryDocumentProvider));
        }

        public async Task<DiscoveryDocumentResponse?> GetDiscoveryDocument()
        {
            if (_value is not null)
                return _value;

            var jwtBearerOptions = _jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);
            _value = await _httpClient.GetDiscoveryDocumentAsync(jwtBearerOptions.MetadataAddress);
            return _value;
        }
    }

    private class OpenApiAuthDocumentTransformer : IOpenApiDocumentTransformer
    {
        private OpenApiSecurityScheme? _securityScheme;

        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var securityScheme = await GetSecurityScheme(context.ApplicationServices);
            if (securityScheme is null)
                return;

            if (document.Components is null)
                document.Components = new OpenApiComponents();

            document.Components.SecuritySchemes.Add(securityScheme.Name, _securityScheme);
        }

        private async Task<OpenApiSecurityScheme?> GetSecurityScheme(IServiceProvider serviceProvider)
        {
            if (_securityScheme is not null)
                return _securityScheme;

            var discoveryDocumentProvider = serviceProvider.GetRequiredService<DiscoveryDocumentProvider>();
            var document = await discoveryDocumentProvider.GetDiscoveryDocument();
            if (document is null || document.AuthorizeEndpoint is null || document.TokenEndpoint is null)
                return null;

            var jwtBearerOptions = serviceProvider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();
            var audience = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme).Audience;
            var scopes = new Dictionary<string, string>();
            if (audience is not null)
                scopes.Add(audience, audience);

            _securityScheme = new OpenApiSecurityScheme()
            {
                Name = "OAuth2",
                Type = SecuritySchemeType.OAuth2,
                Flows = new()
                {
                    AuthorizationCode = new()
                    {
                        AuthorizationUrl = new Uri(document.AuthorizeEndpoint),
                        TokenUrl = new Uri(document.TokenEndpoint),
                        Scopes = scopes,
                    }
                },
            };
            return _securityScheme;
        }
    }

    private class OpenApiAuthOperationTransformer : IOpenApiOperationTransformer
    {
        private static readonly OpenApiResponses _authorizedResponses = new OpenApiResponses()
        {
            ["401"] = new OpenApiResponse { Description = "User not authenticated." },
            ["403"] = new OpenApiResponse { Description = "User not authorized to perform this action." },
        };

        private OpenApiSecurityRequirement? _securityRequirement;

        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (!context.Description.ActionDescriptor.IsAuthorizedOperation())
                return Task.CompletedTask;

            foreach (var response in _authorizedResponses.Where(r => !operation.Responses.ContainsKey(r.Key)))
            {
                operation.Responses.Add(response.Key, response.Value);
            }

            var securityRequirement = GetSecurityRequirement(context.ApplicationServices);
            if (securityRequirement is null)
                return Task.CompletedTask;

            operation.Security.Add(securityRequirement);
            return Task.CompletedTask;
        }

        private OpenApiSecurityRequirement GetSecurityRequirement(IServiceProvider serviceProvider)
        {
            if (_securityRequirement is not null)
                return _securityRequirement;

            var referenceSecurityScheme = new OpenApiSecurityScheme()
            {
                Reference = new()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "OAuth2",
                },
            };

            var jwtBearerOptions = serviceProvider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();
            var audience = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme).Audience;
            var scopes = new List<string>();
            if (audience is not null)
                scopes.Add(audience);

            _securityRequirement = new OpenApiSecurityRequirement()
            {
                { referenceSecurityScheme, scopes }
            };
            return _securityRequirement;
        }
    }
}
