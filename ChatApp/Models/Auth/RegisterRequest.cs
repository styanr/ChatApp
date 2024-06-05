namespace ChatApp.Models.Auth;

public record RegisterRequest(string Email, string Password, string DisplayName, string? Bio, Guid? ProfilePictureId);