using ChatApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Context;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<DirectChatRoom> DirectChatRooms { get; set; }
    public DbSet<GroupChatRoom> GroupChatRooms { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<User>()
            .Property(x => x.Id).HasDefaultValueSql("NEWID()");
        
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email).IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Handle).IsUnique();
        
        modelBuilder.Entity<ChatRoom>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<ChatRoom>()
            .Property(x => x.Id).HasDefaultValueSql("NEWID()");

        modelBuilder.Entity<ChatRoom>()
            .HasMany(x => x.Messages)
            .WithOne(x => x.ChatRoom)
            .HasForeignKey(x => x.ChatRoomId);

        modelBuilder.Entity<DirectChatRoom>()
            .HasIndex(x => new { x.User1Id, x.User2Id }).IsUnique();
        
        modelBuilder.Entity<DirectChatRoom>()
            .HasOne(x => x.User1)
            .WithMany()
            .HasForeignKey(x => x.User1Id)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<DirectChatRoom>()
            .HasOne(x => x.User2)
            .WithMany()
            .HasForeignKey(x => x.User2Id)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<GroupChatRoom>()
            .HasMany(x => x.UserList)
            .WithMany()
            .UsingEntity(x => x.ToTable("GroupChatRoomUsers"));

        modelBuilder.Entity<Contact>()
            .HasOne(x => x.User)
            .WithMany(x => x.Contacts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Contact>()
            .HasOne(x => x.ContactUser)
            .WithMany()
            .HasForeignKey(x => x.ContactId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Contact>()
            .HasKey(x => new { x.UserId, x.ContactId });
        
    }
}