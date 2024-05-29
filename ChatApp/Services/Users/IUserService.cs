using ChatApp.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Services.Users;

public interface IUserService
{
    Task<UserListResponse> GetUsersAsync(UserSearchRequest request, Guid userId);
    Task<UserResponse> GetUserAsync(Guid id, Guid userId);
}