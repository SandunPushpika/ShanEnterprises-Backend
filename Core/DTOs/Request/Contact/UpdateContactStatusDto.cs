using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.Request.Contact;

public class UpdateContactStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
    public string? AdminNotes { get; set; }
}
