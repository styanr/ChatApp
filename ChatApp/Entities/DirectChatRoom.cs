namespace ChatApp.Entities;

public class DirectChatRoom : ChatRoom
{
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    
    public User User1 { get; set; }
    public User User2 { get; set; }
}