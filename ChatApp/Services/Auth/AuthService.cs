using System.Security.Claims;
using ChatApp.Entities;
using ChatApp.Models.Auth;
using ChatApp.Repositories;
using ChatApp.Repositories.Users;

namespace ChatApp.Services.Auth;

public class AuthService : IAuthService
{
    private readonly JwtUtil _jwtUtil;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(JwtUtil jwtUtil, IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _jwtUtil = jwtUtil;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        var result = user is not null && _passwordHasher.VerifyPassword(user.PasswordHash, request.Password);
        
        if (!result)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }
        
        return await GenerateJwtToken(user);
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

        return await GenerateJwtToken(user);
    }

    public async Task<TokenResponse> RefreshTokenAsync(TokenRequest request)
    {
        var principal = _jwtUtil.GetPrincipalFromToken(request.AccessToken);
        
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (userId is null)
        {
            throw new UnauthorizedAccessException("Invalid token");
        }
        
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        
        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid token");
        }
        
        return await GenerateJwtToken(user, false);
    }

    private async Task<TokenResponse> GenerateJwtToken(User user, bool setRefreshTokenExpiry = true)
    {
        var token = _jwtUtil.GenerateJwtToken(user);

        user.RefreshToken = token.RefreshToken;
        if (setRefreshTokenExpiry)
        {
            user.RefreshTokenExpiry = token.RefreshTokenExpiry;
        }

        await _userRepository.UpdateAsync(user);
        
        return token;
    }
}