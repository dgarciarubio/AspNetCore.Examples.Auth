using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AspNetCore.Examples.Auth.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomAuthenticacion(this IServiceCollection services, IConfiguration configuration)
    {
        var authConfig = configuration.GetSection("Authentication");
        if (!authConfig.Exists())
            return services;

        var builder = services.AddAuthentication();

        var bearerConfig = authConfig.GetSection(JwtBearerDefaults.AuthenticationScheme);
        if (!bearerConfig.Exists())
            return services;

        return builder
            .AddJwtBearer(bearerConfig.Bind)
            .Services;
    }
}

