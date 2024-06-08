using System.Security.Claims;
using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Helpers;
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
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetMessages(Guid chatRoomId,
        [FromQuery] PagedRequest request)
    {
        var userId = User.GetUserId();
        var messages = await _messageService.GetMessagesAsync(chatRoomId, userId, request);
        return Ok(messages);
    }
}