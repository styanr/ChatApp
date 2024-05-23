namespace ChatApp.Services.Auth;

public record JwtSettings(
    string Key,
    string Issuer,
    string Audience,
    int DurationInMinutes
);