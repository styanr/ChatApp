using ChatApp.Models.Messages;

namespace ChatApp.Services.ChatRooms;

public interface IChatRoomService
{
    Task<IEnumerable<ChatRoomSummary>> GetAllAsync();
    Task<ChatRoomSummary> CreateGroupChatAsync(ChatRoomCreate chatRoomCreate);
    Task<ChatRoomSummary> AddUsersToChatAsync(Guid chatId, ChatRoomAddUsers chatRoomAddUsers);
    Task<ChatRoomSummary> RemoveUserFromChatAsync(Guid chatId, Guid userId);
    Task<ChatRoomSummary> UpdateGroupChatAsync(Guid chatId, ChatRoomUpdate chatRoomUpdate);
}