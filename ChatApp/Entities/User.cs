using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Entities;

// TODO: Add string length validation
public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; }

    public string? Handle { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string PasswordHash { get; set; }
    
    public string DisplayName { get; set; }
    
    public string? Bio { get; set; }
    
    public string? ProfilePictureUrl { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenExpiry { get; set; }
    
    public List<ChatRoom> ChatRooms { get; set; }
    
    public List<Contact> Contacts { get; set; }
}