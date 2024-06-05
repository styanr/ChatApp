using ChatApp.Entities;
using ChatApp.Managers;
using ChatApp.Models.ChatRooms;
using ChatApp.Models.Messages;

namespace ChatApp.Mapping;

public static class ChatRoomExtensions
{
    public static ChatRoomSummary ToChatRoomSummary(this ChatRoom chatRoom, Guid userId)
    {
        return chatRoom switch
        {
            DirectChatRoom dc => dc.ToChatRoomSummary(userId),
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
            otherUser.ProfilePictureId,
            chatRoom.CreatedAt,
            "direct",
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
            chatRoom.PictureId,
            chatRoom.CreatedAt,
            "group",
            chatRoom.GetLastMessage()?.ToMessageResponse()
        );
    }
    
    private static Message? GetLastMessage(this ChatRoom chatRoom)
    {
        return chatRoom.Messages.MaxBy(x => x.CreatedAt);
    }
    
    public static ChatRoomDetails ToChatRoomDetails(this ChatRoom chatRoom, Guid userId)
    {
        return chatRoom switch
        {
            DirectChatRoom dc => dc.ToChatRoomDetails(userId),
            GroupChatRoom gc => gc.ToChatRoomDetails(),
            _ => throw new ArgumentException("Invalid chat room type")
        };
    }
    
    public static ChatRoomDetails ToChatRoomDetails(this DirectChatRoom chatRoom, Guid userId)
    {
        var otherUser = chatRoom.User1Id == userId ? chatRoom.User2 : chatRoom.User1;
        var user = chatRoom.User1Id == userId ? chatRoom.User1 : chatRoom.User2;
        
        var contact = user.Contacts.FirstOrDefault(c => c.ContactId == otherUser.Id);

        return new ChatRoomDetails
        (
            chatRoom.Id,
            contact?.CustomName ?? otherUser.DisplayName,
            otherUser.Bio,
            otherUser.ProfilePictureId,
            chatRoom.CreatedAt,
            "direct",
            chatRoom.Users.Select(u => u.Id)
        );
    }
    
    public static ChatRoomDetails ToChatRoomDetails(this GroupChatRoom chatRoom)
    {
        return new ChatRoomDetails
        (
            chatRoom.Id,
            chatRoom.Name,
            chatRoom.Description,
            chatRoom.PictureId,
            chatRoom.CreatedAt,
            "group",
            chatRoom.Users.Select(u => u.Id)
        );
    }
}