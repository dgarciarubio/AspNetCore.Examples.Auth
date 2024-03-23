using AspNetCore.Examples.Auth.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Examples.Auth.Api.Controllers;

[ApiController]
[Route("auth")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    public AuthController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [AllowAnonymous]
    [HttpGet("anonymous")]
    public string Anonymous()
    {
        return "Hello kind stranger!";
    }

    [HttpGet("claims")]
    public IEnumerable<Claim> Claims()
    {
        return User.Claims.Select(c => new Claim
        {
            Type = c.Type,
            Value = c.Value,
        });
    }

    [HttpGet("only-admin")]
    [Authorize(Roles = Roles.Admin)]
    public string OnlyAdmin()
    {
        return "Hello admin user!";
    }

    [HttpGet("only-alice")]
    [Authorize(Policy = Policies.IsAlice)]
    public string OnlyAlice()
    {
        return "Hello Alice!";
    }

    [HttpGet("only-bob")]
    [Authorize(Policy = Policies.IsBob)]
    public string OnlyBob()
    {
        return "Hello Bob!";
    }

    [HttpGet("only-recent-login")]
    [Authorize(Policy = Policies.IsRecentLogin)]
    public string OnlyRecentLogin()
    {
        return "Hello recently logged user!";
    }

    [HttpGet("only-same-author/{authorEmail}")]
    public async Task<IActionResult> OnlySameAuthor(string authorEmail)
    {
        var resource = new Resource(authorEmail);
        var result = await _authorizationService.AuthorizeAsync(User, resource, Policies.IsSameAuthor);
        if (!result.Succeeded)
        {
            return Forbid();
        }

        return Ok("You have access to this resource");
    }
}

