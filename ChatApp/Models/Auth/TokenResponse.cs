namespace ChatApp.Models.Auth;

public record TokenResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiry);