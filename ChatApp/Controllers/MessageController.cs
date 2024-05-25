using ChatApp.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/messages")]
public class MessageController : ControllerBase
{
    [HttpGet]
    public async Task<List<Message>?> GetMessages()
    {
        throw new NotImplementedException();
    }
}