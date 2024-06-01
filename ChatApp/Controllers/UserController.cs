﻿using System.Security.Claims;
using ChatApp.Exceptions;
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
        var userId = GetUserId();
        
        var users = await _userService.GetUsersForUserAsync(request, userId);
        return users;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id)
    {
        var userId = GetUserId();

        try
        {
            return await _userService.GetUserForUserAsync(id, userId);
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpGet("current")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = GetUserId();
        
        return await _userService.GetUser(userId);
    }
    
    
    private Guid GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
        if (userId is null)
        {
            throw new UserNotFoundException("User not found");
        }
            
        return Guid.Parse(userId);
    }
    
}