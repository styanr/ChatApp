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
                .ThenInclude(u => u.Contacts)
            .Include(x => x.User2)
                .ThenInclude(u => u.Contacts)
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
                .ThenInclude(x => x.Contacts)
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(x => x.Id == chatRoomId);
    }

    public Task<bool> DirectChatRoomExists(Guid userId, Guid otherUserId)
    {
        return _context.DirectChatRooms
            .AnyAsync(x => (x.User1Id == userId && x.User2Id == otherUserId) || (x.User1Id == otherUserId && x.User2Id == userId));
    }
}