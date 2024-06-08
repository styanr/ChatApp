using ChatApp.Entities;

namespace ChatApp.Repositories.ChatRooms;

public interface IChatRoomRepository : IRepository<ChatRoom>
{
    Task<IEnumerable<DirectChatRoom>> GetDirectChatRoomsAsync(Guid userId);
    Task<IEnumerable<GroupChatRoom>> GetGroupChatRoomsAsync(Guid userId);

    Task<GroupChatRoom?> GetGroupChatRoomAsync(Guid chatRoomId);
    Task<ChatRoom?> GetChatRoomAsync(Guid chatRoomId);
    Task<bool> IsUserInChatRoom(Guid chatRoomId, Guid userId);
    Task<bool> DirectChatRoomExists(Guid userId, Guid otherUserId);
}