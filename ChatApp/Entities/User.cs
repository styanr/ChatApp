using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Entities;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; }

    [MaxLength(50)]
    public string? Handle { get; set; }
    
    [Required]
    [MaxLength(50)]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string PasswordHash { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string DisplayName { get; set; }
    
    [MaxLength(1000)]
    public string? Bio { get; set; }
    
    public Guid? ProfilePictureId { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenExpiry { get; set; }
    
    public List<ChatRoom> ChatRooms { get; set; }
    
    public List<Contact> Contacts { get; set; }
}