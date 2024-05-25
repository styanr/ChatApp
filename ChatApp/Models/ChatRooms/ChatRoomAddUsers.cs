namespace ChatApp.Models.Messages;

public record ChatRoomAddUsers(IEnumerable<Guid> UserIds);