using ChatApp.Models.ChatRooms;
using ChatApp.Models.Messages;

namespace ChatApp.Services.ChatRooms;

public interface IChatRoomService
{
    Task<IEnumerable<ChatRoomSummary>> GetAllAsync(Guid userId);
    Task<ChatRoomDetails> GetChatAsync(Guid userId, Guid chatId);
    Task<ChatRoomDetails> CreateDirectChatAsync(Guid userId, Guid otherUserId);
    Task<ChatRoomDetails> CreateGroupChatAsync(Guid userId, GroupChatRoomCreate groupChatRoomCreate);
    Task<ChatRoomDetails> AddUsersToChatAsync(Guid userId, Guid chatId, ChatRoomAddUsers chatRoomAddUsers);
    Task<ChatRoomDetails> RemoveUserFromChatAsync(Guid userId, Guid chatId, Guid deleteUserId);
    Task<ChatRoomDetails?> LeaveChatAsync(Guid userId, Guid chatId);
    Task<ChatRoomDetails> UpdateGroupChatAsync(Guid userId, Guid chatId, ChatRoomUpdate chatRoomUpdate);
    
    Task<bool> IsUserInChatAsync(Guid userId, Guid chatId);
}