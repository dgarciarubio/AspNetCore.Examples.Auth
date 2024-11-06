using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AspNetCore.Examples.Auth.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var bearerConfig = configuration
            .GetSection($"Authentication:{JwtBearerDefaults.AuthenticationScheme}");

        return services
            .AddAuthentication()
            .AddJwtBearer(bearerConfig.Bind)
            .Services;
    }
}

