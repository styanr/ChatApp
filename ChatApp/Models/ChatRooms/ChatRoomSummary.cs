namespace ChatApp.Models.Messages;

public record ChatRoomSummary(Guid Id, String? Name, String? Description, Guid? PictureId, DateTime CreatedAt, string Type, MessageResponse? LastMessage);