namespace ChatApp.Models.Messages;

public record MessageCreate(string Content, Guid? AttachmentId);