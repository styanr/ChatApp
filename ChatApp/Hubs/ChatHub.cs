using ChatApp.Exceptions;
using ChatApp.Models.Messages;
using ChatApp.Services.ChatRooms;
using ChatApp.Services.Messages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace ChatApp.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatRoomService _chatRoomService;
    private readonly IMessageService _messageService;

    public ChatHub(IChatRoomService chatRoomService, IMessageService messageService)
    {
        _chatRoomService = chatRoomService;
        _messageService = messageService;
    }
    
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine("Connected");
        var userId = GetUserId();
        
        var chatRooms = await _chatRoomService.GetAllAsync(userId);
        
        foreach (var chatRoom in chatRooms)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoom.Id.ToString());
        }
        
        await base.OnConnectedAsync();
    }
    
    public async Task JoinChat(Guid chatId)
    {
        var userId = GetUserId();
        var isUserInChat = await _chatRoomService.IsUserInChatAsync(userId, chatId);

        if (!isUserInChat)
        {
            throw new HubException("User is not a member of the chat");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }
    
    public async Task SendMessage(Guid chatId, MessageCreate message)
    {
        var userId = GetUserId();
        
        var messageResponse =  await _messageService.CreateMessageAsync(chatId, userId, message);
        
        Console.WriteLine("Sending message");
        
        await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", messageResponse);
    }
    
    private Guid GetUserId()
    {
        var userId = Context.UserIdentifier;
        
        if (userId is null)
        {
            throw new UserNotFoundException("User not found");
        }
        
        return Guid.Parse(userId);
    }
}