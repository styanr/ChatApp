using System.Collections;

namespace ChatApp.Entities;

public class GroupChatRoom : ChatRoom
{
    public ICollection<User> Users { get; set; }
    
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? PictureUrl { get; set; }
}