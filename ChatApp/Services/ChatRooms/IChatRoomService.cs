using ChatApp.Models.Messages;

namespace ChatApp.Services.ChatRooms;

public interface IChatRoomService
{
    Task<IEnumerable<ChatRoomSummary>> GetAllAsync(Guid userId);
    Task<ChatRoomSummary> CreateGroupChatAsync(Guid userId, ChatRoomCreate chatRoomCreate);
    Task<ChatRoomSummary> AddUsersToChatAsync(Guid userId, Guid chatId, ChatRoomAddUsers chatRoomAddUsers);
    Task<ChatRoomSummary> RemoveUserFromChatAsync(Guid userId, Guid chatId, Guid deleteUserId);
    Task<ChatRoomSummary> UpdateGroupChatAsync(Guid userId, Guid chatId, ChatRoomUpdate chatRoomUpdate);
    
    Task<bool> IsUserInChatAsync(Guid userId, Guid chatId);
}