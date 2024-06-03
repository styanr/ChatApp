namespace ChatApp.Models.Users;

public record UserUpdate(string DisplayName, string? ProfilePictureUrl, string? Bio, string? Handle);