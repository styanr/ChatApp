using System.Security.Claims;
using ChatApp.Exceptions;
using ChatApp.Helpers;
using ChatApp.Models.Users;
using ChatApp.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<UserListResponse>> GetUsers([FromQuery] UserSearchRequest request)
    {
        var userId = User.GetUserId();

        var users = await _userService.GetUsersForUserAsync(request, userId);
        return users;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id)
    {
        var userId = User.GetUserId();
        
        return await _userService.GetUserForUserAsync(id, userId);
    }

    [HttpGet("current")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = User.GetUserId();

        return await _userService.GetUser(userId);
    }

    [HttpPut("current")]
    public async Task<ActionResult<UserResponse>> UpdateUser([FromBody] UserUpdate request)
    {
        var userId = User.GetUserId();

        return await _userService.UpdateUserAsync(userId, request);
    }
}