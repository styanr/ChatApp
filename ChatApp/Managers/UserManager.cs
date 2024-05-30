using ChatApp.Entities;
using ChatApp.Repositories.Contacts;
using ChatApp.Repositories.Users;

namespace ChatApp.Managers;

public class UserManager : IUserManager
{
    private readonly IUserRepository _userRepository;
    private readonly IContactRepository _contactRepository;

    public UserManager(IUserRepository userRepository, IContactRepository contactRepository)
    {
        _userRepository = userRepository;
        _contactRepository = contactRepository;
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await _userRepository.ExistsAsync(userId);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<Contact?> GetContactAsync(Guid userId, Guid contactId)
    {
        return await _contactRepository.GetByIdWithIncludesAsync(userId, contactId);
    }
}