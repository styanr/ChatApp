using ChatApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Context;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
}