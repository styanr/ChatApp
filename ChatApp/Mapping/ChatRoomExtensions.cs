using ChatApp.Entities;
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

        return new ChatRoomSummary
        (
            chatRoom.Id,
            otherUser.DisplayName,
            otherUser.Bio,
            otherUser.ProfilePictureUrl,
            chatRoom.CreatedAt,
            chatRoom.Messages.MaxBy(x => x.CreatedAt)?.ToMessageResponse()
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
            chatRoom.Messages.MaxBy(x => x.CreatedAt)?.ToMessageResponse()
        );
    }
}