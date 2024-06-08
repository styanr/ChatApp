using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Entities;

public class GroupChatRoom : ChatRoom
{
    public ICollection<User> UserList { get; set; } = new List<User>();
    
    [MaxLength(100)]
    public string Name { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    public Guid? PictureId { get; set; }
    public override ICollection<User> Users => UserList;
}