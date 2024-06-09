using ChatApp.Exceptions;
using ChatApp.Helpers;
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
    
    private static readonly ConnectionMapping<Guid> Connections = new();
    public ChatHub(IChatRoomService chatRoomService, IMessageService messageService)
    {
        _chatRoomService = chatRoomService;
        _messageService = messageService;
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        
        Connections.Add(userId, Context.ConnectionId);
        
        var chatRooms = await _chatRoomService.GetAllAsync(userId);
        
        foreach (var chatRoom in chatRooms)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoom.Id.ToString());
        }
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        
        Connections.Remove(userId, Context.ConnectionId);
        
        await base.OnDisconnectedAsync(exception);
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
    
    public async Task CreateDirectChatRoom(Guid otherUserId)
    {
        var userId = GetUserId();
        
        var chatRoom = await _chatRoomService.CreateDirectChatAsync(userId, otherUserId);
        
        var otherUserConnectionIds = Connections.GetConnections(otherUserId);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, chatRoom.Id.ToString());
        await Clients.Caller.SendAsync("ReceiveChatRoom", chatRoom);
        
        foreach (var connectionId in otherUserConnectionIds)
        {
            await Groups.AddToGroupAsync(connectionId, chatRoom.Id.ToString());
            await Clients.Client(connectionId).SendAsync("ReceiveChatRoom", chatRoom);
        }
    }
    
    public async Task CreateGroupChatRoom(GroupChatRoomCreate groupChatRoomCreate)
    {
        var userId = GetUserId();
        
        var chatRoom = await _chatRoomService.CreateGroupChatAsync(userId, groupChatRoomCreate);
        await Groups.AddToGroupAsync(Context.ConnectionId, chatRoom.Id.ToString());
        await Clients.Caller.SendAsync("ReceiveChatRoom", chatRoom);
    }
    
    public async Task UpdateGroupChatRoom(Guid chatId, ChatRoomUpdate chatRoomUpdate)
    {
        var userId = GetUserId();
        
        var chatRoom = await _chatRoomService.UpdateGroupChatAsync(userId, chatId, chatRoomUpdate);
        
        var otherUserConnectionIds = Connections.GetConnections(chatRoom.UserIds);
        
        foreach (var connectionId in otherUserConnectionIds)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveChatRoom", chatRoom);
        }
    }
    
    public async Task AddUsersToGroupChatRoom(Guid chatId, ChatRoomAddUsers chatRoomAddUsers)
    {
        var userId = GetUserId();
        
        var chatRoom = await _chatRoomService.AddUsersToChatAsync(userId, chatId, chatRoomAddUsers);

        List<string> otherUserConnectionIds = [];
        
        foreach (var addUserId in chatRoomAddUsers.UserIds)
        {
            otherUserConnectionIds.AddRange(Connections.GetConnections(addUserId)); 
        }
        
        foreach (var connectionId in otherUserConnectionIds)
        {
            await Groups.AddToGroupAsync(connectionId, chatRoom.Id.ToString());
            await Clients.Client(connectionId).SendAsync("ReceiveChatRoom", chatRoom);
        }
    }
    
    public async Task LeaveChatRoom(Guid chatId)
    {
        var userId = GetUserId();
        
        var chatRoom = await _chatRoomService.LeaveChatAsync(userId, chatId);
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        
        await Clients.Caller.SendAsync("ReceiveChatRoom", chatRoom);
    }
    
    public async Task SendMessage(Guid chatId, MessageCreate message)
    {
        var userId = GetUserId();
        
        var messageResponse =  await _messageService.CreateMessageAsync(chatId, userId, message);
        
        Console.WriteLine("Sending message");
        
        await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", messageResponse);
    }
    
    public async Task EditMessage(Guid messageId, MessageUpdate message)
    {
        var userId = GetUserId();
        
        var messageResponse = await _messageService.EditMessageAsync(messageId, userId, message);
        
        await Clients.Group(messageResponse.ChatRoomId.ToString()).SendAsync("ReceiveEditMessage", messageResponse);
    }
    
    public async Task DeleteMessage(Guid messageId)
    {
        var userId = GetUserId();
        
        var (chatRoomId, deletedMessageId) = await _messageService.DeleteMessageAsync(messageId, userId);
        
        await Clients.All.SendAsync("ReceiveDeleteMessage", chatRoomId, deletedMessageId);
    }
    
    private Guid GetUserId()
    {
        var userId = Context.UserIdentifier;
        
        if (userId is null)
        {
            throw new UserIdNotFound();
        }
        
        return Guid.Parse(userId);
    }
}