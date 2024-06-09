using ChatApp.Entities;
using ChatApp.Models.Messages;

namespace ChatApp.Mapping;

public static class MessageExtensions
{
    public static MessageResponse ToMessageResponse(this Message message)
    {
        return new MessageResponse
        (
            message.Id,
            message.ChatRoomId,
            message.AuthorId,
            message.IsDeleted ? "" : message.Content,
            message.IsDeleted ? null : message.AttachmentId,
            message.CreatedAt,
            message.EditedAt,
            message.IsDeleted
        );
    }
}