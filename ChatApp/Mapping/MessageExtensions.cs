﻿using ChatApp.Entities;
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
            message.Content,
            message.AttachmentId,
            message.CreatedAt,
            message.EditedAt,
            message.IsDeleted
        );
    }
}