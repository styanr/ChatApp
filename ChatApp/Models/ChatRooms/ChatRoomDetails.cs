namespace ChatApp.Models.ChatRooms;

public record ChatRoomDetails(Guid Id, String? Name, String? Description, Guid? PictureId, DateTime CreatedAt, string Type, IEnumerable<Guid> UserIds);