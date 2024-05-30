using ChatApp.Entities;

namespace ChatApp.Managers;

public interface IUserManager
{
    Task<bool> UserExistsAsync(Guid userId);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByEmailAsync(String email);
    Task<Contact?> GetContactAsync(Guid userId, Guid contactId);
}