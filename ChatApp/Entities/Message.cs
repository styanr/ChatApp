using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Entities;

public class Message
{
    public Guid Id { get; set; }
    
    public Guid ChatRoomId { get; set; }
    public ChatRoom ChatRoom { get; set; }
    
    public Guid AuthorId { get; set; }
    public User Author { get; set; }
    
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }
    public Boolean IsDeleted { get; set; } = false;
}