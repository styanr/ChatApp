namespace ChatApp.Models.Users;

public record UserUpdate(string DisplayName, Guid? ProfilePictureId, string? Bio, string? Handle);