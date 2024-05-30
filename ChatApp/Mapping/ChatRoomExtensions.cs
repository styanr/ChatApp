using ChatApp.Entities;
using ChatApp.Managers;
using ChatApp.Models.Messages;

namespace ChatApp.Mapping;

public static class ChatRoomExtensions
{
    public static ChatRoomSummary ToChatRoomSummary(this ChatRoom chatRoom)
    {
        return chatRoom switch
        {
            DirectChatRoom dc => dc.ToChatRoomSummary(),
            GroupChatRoom gc => gc.ToChatRoomSummary(),
            _ => throw new ArgumentException("Invalid chat room type")
        };
    }
    public static ChatRoomSummary ToChatRoomSummary(this DirectChatRoom chatRoom, Guid userId)
    {
        var otherUser = chatRoom.User1Id == userId ? chatRoom.User2 : chatRoom.User1;
        var user = chatRoom.User1Id == userId ? chatRoom.User1 : chatRoom.User2;
        
        var contact = user.Contacts.FirstOrDefault(c => c.ContactId == otherUser.Id);

        return new ChatRoomSummary
        (
            chatRoom.Id,
            contact?.CustomName ?? otherUser.DisplayName,
            otherUser.Bio,
            otherUser.ProfilePictureUrl,
            chatRoom.CreatedAt,
            chatRoom.GetLastMessage()?.ToMessageResponse()
        );
    }
    
    public static ChatRoomSummary ToChatRoomSummary(this GroupChatRoom chatRoom)
    {
        return new ChatRoomSummary
        (
            chatRoom.Id,
            chatRoom.Name,
            chatRoom.Description,
            chatRoom.PictureUrl,
            chatRoom.CreatedAt,
            chatRoom.GetLastMessage()?.ToMessageResponse()
        );
    }
    
    private static Message? GetLastMessage(this ChatRoom chatRoom)
    {
        return chatRoom.Messages.MaxBy(x => x.CreatedAt);
    }
}