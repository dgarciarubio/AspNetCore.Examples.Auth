using AspNetCore.Examples.Auth.Api.Authorization;
using AspNetCore.Examples.Auth.Api.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCore.Examples.Auth.Api.Extensions;

public static class AuthorizationExtensions
{
    private const string AliceEmail = "AliceSmith@email.com";
    private const string BobEmail = "BobSmith@email.com";

    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAuthorizationHandler, AuthTimeRequirementHandler>()
            .AddSingleton<IAuthorizationHandler, SameAuthorRequirementHandler>()
            .AddAuthorization(options =>
            {
                options.AddPolicy(Policies.IsAlice, options =>
                {
                    options.RequireClaim(ClaimTypes.Email, AliceEmail);
                });
                options.AddPolicy(Policies.IsBob, options =>
                {
                    options.RequireClaim(ClaimTypes.Email, BobEmail);
                });
                options.AddPolicy(Policies.IsRecentLogin, options =>
                {
                    options.AddRequirements(new AuthTimeRequirement(MaxTimeAgo: TimeSpan.FromMinutes(5)));
                });
                options.AddPolicy(Policies.IsSameAuthor, options =>
                {
                    options.AddRequirements(new SameAuthorRequirement());
                });
            });
    }

    public static WebApplication UseCustomAuthorization(this WebApplication app)
    {
        //Custom middleware to emulate roles in identity server users
        app.Use((context, next) =>
        {
            if (context.User.FindFirstValue(ClaimTypes.Email) == AliceEmail)
            {
                context.User.AddIdentity(new ClaimsIdentity(
                [
                    new System.Security.Claims.Claim(ClaimTypes.Role, Roles.Admin),
                ]));
            }

            return next();
        });

        app.UseAuthorization();

        return app;
    }
}

