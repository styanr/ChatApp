using System.ComponentModel.DataAnnotations;

namespace ChatApp.Entities;

public class Contact
{
    public Guid UserId { get; set; }
    public Guid ContactId { get; set; }
    
    [MaxLength(100)]
    public string CustomName { get; set; }
    
    public User User { get; set; }
    public User ContactUser { get; set; }
}