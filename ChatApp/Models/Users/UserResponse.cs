namespace ChatApp.Models.Users;

public record UserResponse(Guid Id, string? Handle, string DisplayName, string? Bio, string? ProfilePictureUrl, bool IsContact);