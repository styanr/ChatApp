using ChatApp.Context;
using ChatApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories.ChatRooms;

public class ChatRoomRepository(ChatDbContext context) : Repository<ChatRoom>(context), IChatRoomRepository
{
    public async Task<IEnumerable<DirectChatRoom>> GetDirectChatRoomsAsync(Guid userId)
    {
        return await _context.DirectChatRooms
            .Include(x => x.User1)
            .Include(x => x.User2)
            .Where(x => x.User1Id == userId || x.User2Id == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<GroupChatRoom>> GetGroupChatRoomsAsync(Guid userId)
    {
        return await _context.GroupChatRooms
            .Include(x => x.UserList)
            .Where(x => x.UserList.Any(u => u.Id == userId))
            .ToListAsync();
    }

    public Task<GroupChatRoom?> GetGroupChatRoomAsync(Guid chatRoomId)
    {
        return _context.GroupChatRooms
            .Include(x => x.UserList)
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(x => x.Id == chatRoomId);
    }
}