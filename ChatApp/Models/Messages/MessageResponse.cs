namespace ChatApp.Models.Messages;

public record MessageResponse(Guid Id, Guid ChatRoomId, Guid AuthorId, string Content, DateTime CreatedAt, DateTime? EditedAt, Boolean IsDeleted);