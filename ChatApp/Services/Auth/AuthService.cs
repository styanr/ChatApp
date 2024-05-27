using System.Security.Claims;
using ChatApp.Entities;
using ChatApp.Models.Auth;
using ChatApp.Repositories.Users;

namespace ChatApp.Services.Auth;

public class AuthService : IAuthService
{
    private readonly JwtUtil _jwtUtil;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(JwtUtil jwtUtil, IUserRepository userRepository, IPasswordHasher passwordHasher, IHttpContextAccessor httpContextAccessor)
    {
        _jwtUtil = jwtUtil;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email)
                   ?? throw new UnauthorizedAccessException("Invalid email or password");

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        return await GenerateJwtTokenAsync(user);
    }

    public async Task<TokenResponse> RegisterAsync(RegisterRequest request)
    {
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            Email = request.Email,
            DisplayName = request.DisplayName,
            Bio = request.Bio,
            ProfilePictureUrl = request.ProfilePictureUrl,
            PasswordHash = passwordHash
        };

        await _userRepository.AddAsync(user);
        return await GenerateJwtTokenAsync(user);
    }

    public async Task<TokenResponse> RefreshTokenAsync(TokenRequest request)
    {
        var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"]
                           ?? throw new UnauthorizedAccessException("Invalid token");

        var principal = _jwtUtil.GetPrincipalFromToken(request.AccessToken);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? throw new UnauthorizedAccessException("Invalid token");

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId))
                   ?? throw new UnauthorizedAccessException("Invalid token");

        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid token");
        }

        return await GenerateJwtTokenAsync(user, false);
    }

    private async Task<TokenResponse> GenerateJwtTokenAsync(User user, bool setRefreshTokenExpiry = true)
    {
        var tokens = _jwtUtil.GenerateTokens(user);

        user.RefreshToken = tokens.refreshToken;
        if (setRefreshTokenExpiry)
        {
            user.RefreshTokenExpiry = tokens.refreshTokenExpiry;
        }

        await _userRepository.UpdateAsync(user);
        SetRefreshTokenCookie(tokens.refreshToken, tokens.refreshTokenExpiry);

        return new TokenResponse(tokens.accessToken);
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expiry)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expiry
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
