using ChatApp.Entities;
using ChatApp.Mapping;
using ChatApp.Models;
using ChatApp.Models.Messages;
using ChatApp.Repositories.ChatRooms;
using ChatApp.Repositories.Messages;
using ChatApp.Services.ChatRooms;

namespace ChatApp.Services.Messages;

public class MessageService : IMessageService
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository, IChatRoomRepository chatRoomRepository)
    {
        _messageRepository = messageRepository;
        _chatRoomRepository = chatRoomRepository;
    }
    
    public async Task<IEnumerable<MessageResponse>> GetMessagesAsync(Guid chatRoomId, Guid userId, PagedRequest request)
    {
        var chatRoom = await GetChatRoomAsync(chatRoomId, userId);
        
        var messages = chatRoom.Messages
            .OrderByDescending(m => m.CreatedAt)
            .Skip(request.PageSize * (request.Page - 1))
            .Take(request.PageSize)
            .ToList();
        
        var orderedMessages = messages.OrderBy(m => m.CreatedAt).Select(m => m.ToMessageResponse()).ToList();
        
        return orderedMessages;
    }

    public async Task<MessageResponse> CreateMessageAsync(Guid chatRoomId, Guid userId, MessageCreate message)
    {
        // TODO: need a better way to check if user is a member of chat room
        var chatRoom = await GetChatRoomAsync(chatRoomId, userId);
        
        var newMessage = new Message
        {
            ChatRoomId = chatRoomId,
            AuthorId = userId,
            Content = message.Content,
            AttachmentId = message.AttachmentId
        };
        
        await _messageRepository.AddAsync(newMessage);
        
        return newMessage.ToMessageResponse();
    }

    public async Task<MessageResponse> EditMessageAsync(Guid messageId, Guid userId, MessageUpdate message)
    {
        var existingMessage = await _messageRepository.GetByIdAsync(messageId);
        
        if (existingMessage == null)
        {
            throw new Exception($"Message with ID {messageId} not found.");
        }
        
        if (existingMessage.AuthorId != userId)
        {
            throw new Exception($"User with ID {userId} is not the author of message with ID {messageId}.");
        }
        
        existingMessage.Content = message.Content;
        existingMessage.EditedAt = DateTime.UtcNow;
        
        await _messageRepository.UpdateAsync(existingMessage);

        return existingMessage.ToMessageResponse();
    }

    public async Task<(Guid, Guid)> DeleteMessageAsync(Guid messageId, Guid userId)
    {
        var existingMessage = await _messageRepository.GetByIdAsync(messageId);
        
        if (existingMessage == null)
        {
            throw new Exception($"Message with ID {messageId} not found.");
        }
        
        if (existingMessage.AuthorId != userId)
        {
            throw new Exception($"User with ID {userId} is not the author of message with ID {messageId}.");
        }
        
        existingMessage.IsDeleted = true;
        
        await _messageRepository.UpdateAsync(existingMessage);
        
        return (existingMessage.ChatRoomId, existingMessage.Id);
    }
    
    private async Task<ChatRoom> GetChatRoomAsync(Guid chatRoomId, Guid userId)
    {
        var chatRoom = await _chatRoomRepository.GetChatRoomAsync(chatRoomId);
        if (chatRoom == null)
        {
            throw new Exception($"Chat room with ID {chatRoomId} not found.");
        }
        
        if (chatRoom.Users.All(u => u.Id != userId))
        {
            throw new Exception($"User with ID {userId} is not a member of chat room with ID {chatRoomId}.");
        }

        return chatRoom;
    }
}