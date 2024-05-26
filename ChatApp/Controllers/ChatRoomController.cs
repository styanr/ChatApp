using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatApp.Exceptions;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Models.Messages;
using ChatApp.Services.ChatRooms;

namespace ChatApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatRoomController : ControllerBase
{
    private readonly IChatRoomService _chatRoomService;

    public ChatRoomController(IChatRoomService chatRoomService)
    {
        _chatRoomService = chatRoomService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatRoomSummary>>> GetAll()
    {
        try
        {
            var userId = GetUserId();
            var chatRooms = await _chatRoomService.GetAllAsync(userId);
            return Ok(chatRooms);
        }
        catch (UserNotFoundException e)
        {
            return Unauthorized(new ErrorResponse(e.Message));
        }
    }

    [HttpPost("group")]
    public async Task<ActionResult<ChatRoomSummary>> CreateGroupChat([FromBody] ChatRoomCreate chatRoomCreate)
    {
        try
        {
            var userId = GetUserId();
            var chatRoomSummary = await _chatRoomService.CreateGroupChatAsync(userId, chatRoomCreate);
            return CreatedAtAction(nameof(GetAll), new { id = chatRoomSummary.Id }, chatRoomSummary);
        }
        catch (UserNotFoundException e)
        {
            return Unauthorized(new ErrorResponse(e.Message));
        }
    }

    [HttpPost("{chatId}/users")]
    public async Task<ActionResult<ChatRoomSummary>> AddUsersToChat(Guid chatId, [FromBody] ChatRoomAddUsers chatRoomAddUsers)
    {
        try
        {
            var userId = GetUserId();
            var chatRoomSummary = await _chatRoomService.AddUsersToChatAsync(userId, chatId, chatRoomAddUsers);
            return Ok(chatRoomSummary);
        }
        catch (ChatRoomNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{chatId}/users/{deleteUserId}")]
    public async Task<ActionResult<ChatRoomSummary>> RemoveUserFromChat(Guid chatId, Guid deleteUserId)
    {
        try
        {
            var userId = GetUserId();
            var chatRoomSummary = await _chatRoomService.RemoveUserFromChatAsync(userId, chatId, deleteUserId);
            return Ok(chatRoomSummary);
        }
        catch (ChatRoomNotFoundException e)
        {
            return NotFound(new ErrorResponse(e.Message));
        }
        catch (UserNotFoundException e)
        {
            return NotFound(new ErrorResponse(e.Message));
        }
    }

    [HttpPut("{chatId}")]
    public async Task<ActionResult<ChatRoomSummary>> UpdateGroupChat(Guid chatId, [FromBody] ChatRoomUpdate chatRoomUpdate)
    {
        try
        {
            var userId = GetUserId();
            var chatRoomSummary = await _chatRoomService.UpdateGroupChatAsync(userId, chatId, chatRoomUpdate);
            return Ok(chatRoomSummary);
        }
        catch (ChatRoomNotFoundException e)
        {
            return NotFound(new ErrorResponse(e.Message));
        }
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