namespace ChatApp.Services.Auth;

public record JwtOptions(
    string Key,
    string Issuer,
    string Audience,
    int DurationInMinutes,
    int RefreshTokenDurationInDays
);