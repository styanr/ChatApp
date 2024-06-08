using System.Security.Claims;
using ChatApp.Exceptions;
using ChatApp.Helpers;
using ChatApp.Models;
using ChatApp.Models.Users;
using ChatApp.Services.Contacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/contacts")]
[Authorize]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }
    
    [HttpGet]
    public async Task<ActionResult<UserListResponse>> GetContactsAsync([FromQuery] PagedRequest pagedRequest)
    {
        var userId = User.GetUserId();
        
        var userResponses = await _contactService.GetContactsAsync(userId, pagedRequest);

        return userResponses;
    }
    
    [HttpPost]
    public async Task<ActionResult<UserResponse>> AddContactAsync([FromBody] UserContactAdd userContactAdd)
    {
        var userId = User.GetUserId();
        try
        {
            var userResponse = await _contactService.AddContactAsync(userId, userContactAdd);
            
            return userResponse;
        }
        catch (UserNotFoundException e)
        {
            return NotFound(new ErrorResponse(e.Message));
        }
    }
    
    [HttpDelete("{contactId}")]
    public async Task<IActionResult> RemoveContactAsync(Guid contactId)
    {
        var userId = User.GetUserId();
        try
        {
            await _contactService.RemoveContactAsync(userId, contactId);
            return NoContent();
        }
        catch (UserNotFoundException e)
        {
            return NotFound(new ErrorResponse(e.Message));
        }
        catch (UnauthorizedAccessException e)
        {
            return NotFound();
        }
    }
    
    [HttpPut("{contactId}")]
    public async Task<ActionResult<UserResponse>> UpdateContactAsync(Guid contactId, [FromBody] UserContactUpdate userContactUpdate)
    {
        var userId = User.GetUserId();
        try
        {
            var userResponse = await _contactService.UpdateContactAsync(userId, contactId, userContactUpdate);
            return userResponse;
        }
        catch (ContactNotFoundException e)
        {
            return NotFound(new ErrorResponse(e.Message));
        }
        catch (UnauthorizedAccessException e)
        {
            return NotFound();
        }
    }
}