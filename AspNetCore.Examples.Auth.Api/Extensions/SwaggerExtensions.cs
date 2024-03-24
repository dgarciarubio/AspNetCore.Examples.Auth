using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCore.Examples.Auth.Api.Extensions;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var swaggerGenConfig = configuration.GetSection("Swagger:SwaggerGen");
        if (!swaggerGenConfig.Exists())
            return services;

        return services
            .AddSwaggerGen(options =>
            {
                options.OperationFilter<AuthorizedOperationFilter>(swaggerGenConfig);

                var securityDefinitionsConfig = swaggerGenConfig
                    .GetSection("SecurityDefinitions")
                    .GetChildren();
                foreach (var securityDefinitionConfig in securityDefinitionsConfig)
                {
                    options.AddSecurityDefinition(securityDefinitionConfig.Key, securityDefinitionConfig.Get<OpenApiSecurityScheme>());
                }
            });
    }

    public static WebApplication UseCustomSwagger(this WebApplication app, IConfiguration configuration)
    {
        var swaggerConfig = configuration.GetSection("Swagger");
        if (!swaggerConfig.Exists())
            return app;

        app.UseSwagger();

        var swaggerUIConfig = swaggerConfig.GetSection("SwaggerUI");
        if (!swaggerUIConfig.Exists())
            return app;

        app.UseSwaggerUI(swaggerUIConfig.Bind);

        return app;
    }

    private sealed class AuthorizedOperationFilter : IOperationFilter
    {
        private readonly OpenApiSecurityRequirement _securityRequirement;

        private static readonly OpenApiResponses _authorizedResponses = new OpenApiResponses()
        {
            ["401"] = new OpenApiResponse { Description = "User not authenticated." },
            ["403"] = new OpenApiResponse { Description = "User not authorized to perform this action." },
        };

        public AuthorizedOperationFilter(IConfigurationSection swaggerGenConfig)
        {
            _securityRequirement = GetSecurityRequirement(swaggerGenConfig);

            static OpenApiSecurityRequirement GetSecurityRequirement(IConfigurationSection swaggerGenConfig)
            {
                var securitySchemes = swaggerGenConfig
                    .GetSection("SecurityRequirements")
                    .GetChildren()
                    .Select(c => new
                    {
                        Scheme = c.GetSection("SecurityScheme").Get<OpenApiSecurityScheme>(),
                        Scopes = c.GetSection("Scopes").Get<string[]>() ?? Array.Empty<string>(),
                    });

                var securityRequirement = new OpenApiSecurityRequirement();
                foreach (var securityScheme in securitySchemes.Where(s => s.Scheme is not null))
                {
                    securityRequirement.Add(securityScheme.Scheme!, securityScheme.Scopes);
                }
                return securityRequirement;
            }
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!IsAuthorizedOperation(context))
                return;

            operation.Security.Add(_securityRequirement);
            foreach (var response in _authorizedResponses)
            {
                if (!operation.Responses.ContainsKey(response.Key))
                {
                    operation.Responses.Add(response.Key, response.Value);
                }
            }

            static bool IsAuthorizedOperation(OperationFilterContext context)
            {
                if (context.MethodInfo.DeclaringType is null)
                    return false;

                var attributes = context.MethodInfo.DeclaringType
                    .GetCustomAttributes(inherit: true)
                    .Union(context.MethodInfo.GetCustomAttributes(inherit: true));

                bool authorize = attributes.OfType<AuthorizeAttribute>().Any();
                bool allowAnonimous = attributes.OfType<AllowAnonymousAttribute>().Any();

                return authorize && !allowAnonimous;
            }
        }
    }
}
