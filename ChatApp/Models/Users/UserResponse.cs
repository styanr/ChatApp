namespace ChatApp.Models.Users;

public record UserResponse(Guid Id, string? Handle, string DisplayName, string? Bio, Guid? ProfilePictureId, bool IsContact);