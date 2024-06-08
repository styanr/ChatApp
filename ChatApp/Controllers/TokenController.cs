using ChatApp.Models;
using ChatApp.Models.Auth;
using ChatApp.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/token")]
public class TokenController : ControllerBase
{
    private readonly IAuthService _authService;

    public TokenController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<ActionResult<TokenResponse>> Refresh()
    {
        var header = Request.Headers["Authorization"];

        if (header.Count == 0)
        {
            return Unauthorized(new ErrorResponse("No token provided"));
        }

        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var request = new TokenRequest(token);
        return await _authService.RefreshTokenAsync(request);
    }
}