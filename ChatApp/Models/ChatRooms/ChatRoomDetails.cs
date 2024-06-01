namespace ChatApp.Models.ChatRooms;

public record ChatRoomDetails(Guid Id, String? Name, String? Description, String? PictureUrl, DateTime CreatedAt, string Type, IEnumerable<Guid> UserIds);