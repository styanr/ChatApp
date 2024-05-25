namespace ChatApp.Models.Messages;

public record ChatRoomSummary(Guid Id, String Name, String? Description, String? PictureUrl, DateTime CreatedAt, MessageResponse? LastMessage);