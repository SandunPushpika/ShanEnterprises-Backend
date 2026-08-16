using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.Request.Contact;

public class ContactRequestCreateDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MaxLength(500)]
    public string Subject { get; set; } = string.Empty;
    [Required]
    public string Message { get; set; } = string.Empty;
}
