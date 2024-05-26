using System.Security.Claims;
using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Models;
using ChatApp.Models.Messages;
using ChatApp.Services.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }
    [HttpGet("{chatRoomId}")]
    
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetMessages(Guid chatRoomId, [FromQuery] PagedRequest request)
    {
        try
        {
            var userId = GetUserId();
            var messages = await _messageService.GetMessagesAsync(chatRoomId, userId, request);
            return Ok(messages);
        }
        catch (UserNotFoundException e)
        {
            return Unauthorized(new ErrorResponse(e.Message));
        }
    }
    
    // TODO: extract this to a service
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