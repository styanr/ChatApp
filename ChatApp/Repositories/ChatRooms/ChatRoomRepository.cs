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
            .Include(x => x.Messages)
            .Where(x => x.User1Id == userId || x.User2Id == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<GroupChatRoom>> GetGroupChatRoomsAsync(Guid userId)
    {
        return await _context.GroupChatRooms
            .Include(x => x.UserList)
            .Include(x => x.Messages)
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

    public async Task<ChatRoom?> GetChatRoomAsync(Guid chatRoomId)
    {
        var chatRoom = await _context.ChatRooms
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(x => x.Id == chatRoomId);

        if (chatRoom is GroupChatRoom groupChatRoom)
        {
            await _context.Entry(groupChatRoom)
                .Collection(x => x.UserList)
                .Query()
                .Include(x => x.Contacts)
                .LoadAsync();
        }
        else if (chatRoom is DirectChatRoom directChatRoom)
        {
            await _context.Entry(directChatRoom)
                .Reference(x => x.User1)
                .Query()
                .Include(x => x.Contacts)
                .LoadAsync();

            await _context.Entry(directChatRoom)
                .Reference(x => x.User2)
                .Query()
                .Include(x => x.Contacts)
                .LoadAsync();
        }

        return chatRoom;
    }

    public Task<bool> DirectChatRoomExists(Guid userId, Guid otherUserId)
    {
        return _context.DirectChatRooms
            .AnyAsync(x => (x.User1Id == userId && x.User2Id == otherUserId) || (x.User1Id == otherUserId && x.User2Id == userId));
    }
}