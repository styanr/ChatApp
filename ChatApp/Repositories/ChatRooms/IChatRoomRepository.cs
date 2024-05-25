using ChatApp.Entities;

namespace ChatApp.Repositories.ChatRooms;

public interface IChatRoomRepository : IRepository<ChatRoom>
{
    Task<IEnumerable<DirectChatRoom>> GetDirectChatRoomsAsync(Guid userId);
    Task<IEnumerable<GroupChatRoom>> GetGroupChatRoomsAsync(Guid userId);

    Task<GroupChatRoom?> GetGroupChatRoomAsync(Guid chatRoomId);
}