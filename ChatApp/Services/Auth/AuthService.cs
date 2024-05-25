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
        
        return new TokenResponse(_jwtUtil.GenerateJwtToken(user));
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

        return new TokenResponse(_jwtUtil.GenerateJwtToken(user));
    }
}