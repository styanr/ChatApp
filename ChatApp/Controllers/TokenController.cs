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
    public async Task<ActionResult<TokenResponse>> Refresh([FromBody] TokenRequest request)
    {
        try
        {
            return await _authService.RefreshTokenAsync(request);
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new ErrorResponse(e.Message));
        }
    }
}