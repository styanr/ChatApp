using ChatApp.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Services.Users;

public interface IUserService
{
    Task<UserListResponse> GetUsersForUserAsync(UserSearchRequest request, Guid userId);
    Task<UserResponse> GetUserForUserAsync(Guid id, Guid userId);
    Task<UserResponse> GetUser(Guid id);
    Task<UserResponse> UpdateUserAsync(Guid id, UserUpdate request);
}