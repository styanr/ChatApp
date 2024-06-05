namespace ChatApp.Models.Messages;

public record MessageCreate(string Content, List<Guid>? AttachmentIds);