using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCore.Examples.Auth.Api.Authorization.Requirements;

public class SameAuthorRequirement : IAuthorizationRequirement
{
}

public class SameAuthorRequirementHandler : AuthorizationHandler<SameAuthorRequirement, Resource>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAuthorRequirement requirement, Resource resource)
    {
        if (context.User.FindFirstValue(ClaimTypes.Email) == resource.AuthorEmail)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}