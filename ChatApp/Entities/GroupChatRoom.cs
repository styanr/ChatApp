using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Entities;

public class GroupChatRoom : ChatRoom
{
    public ICollection<User> UserList { get; set; } = new List<User>();
    
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? PictureUrl { get; set; }
    public override ICollection<User> Users => UserList;
}