using ChatApp.Context;
using ChatApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories.Users;

public class UserRepository(ChatDbContext context) : Repository<User>(context), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}