using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Entities;

public class DirectChatRoom : ChatRoom
{
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    
    public User User1 { get; set; }
    public User User2 { get; set; }

    public override ICollection<User> Users => new List<User> { User1, User2 };
}