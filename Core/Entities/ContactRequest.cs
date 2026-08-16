using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("contact_requests")]
public class ContactRequest
{
    [Key]
    public int Id { get; set; }
    public long? UserId { get; set; }
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    [Required, MaxLength(500)]
    public string Subject { get; set; } = string.Empty;
    [Required]
    public string Message { get; set; } = string.Empty;
    [MaxLength(50)]
    public string Status { get; set; } = "NEW"; // NEW, IN_PROGRESS, RESOLVED, CLOSED
    public string? AdminNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public virtual User? User { get; set; }
}
