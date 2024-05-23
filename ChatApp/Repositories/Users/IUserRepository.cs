using ChatApp.Entities;

namespace ChatApp.Repositories.Users;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}