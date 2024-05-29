namespace ChatApp.Entities;

public class Contact
{
    public Guid UserId { get; set; }
    public Guid ContactId { get; set; }
    
    public string CustomName { get; set; }
    
    public User User { get; set; }
    public User ContactUser { get; set; }
}