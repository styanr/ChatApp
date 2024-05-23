using ChatApp.Models.Auth;

namespace ChatApp.Services.Auth;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginRequest request);
    Task<TokenResponse> RegisterAsync(RegisterRequest request);
}