using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCore.Examples.Auth.Api.Authorization.Requirements;

public record class AuthTimeRequirement(TimeSpan MaxTimeAgo) : IAuthorizationRequirement
{
}

public class AuthTimeRequirementHandler : AuthorizationHandler<AuthTimeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthTimeRequirement requirement)
    {
        if (TryGetAuthTime(context, out var authTime) &&
            DateTime.UtcNow - authTime < requirement.MaxTimeAgo)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private bool TryGetAuthTime(AuthorizationHandlerContext context, out DateTime? authTime)
    {
        authTime = null;
        var authTimeValue = context.User.FindFirstValue("auth_time");
        if (!double.TryParse(authTimeValue, out var authTimeSeconds))
        {
            return false;
        }

        authTime = DateTime.UnixEpoch.AddSeconds(authTimeSeconds);
        return true;
    }
}
