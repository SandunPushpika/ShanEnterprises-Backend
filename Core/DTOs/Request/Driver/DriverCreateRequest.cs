using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.Request.Driver;

public class DriverCreateRequest
{
    [Required]
    [MaxLength(100)]
    public string LicenseNumber { get; set; } = null!;

    [Required]
    public DateOnly LicenseExpiryDate { get; set; }

    [Range(0, 60)]
    public int YearsOfExperience { get; set; } = 0;

    public string? LicenseDocumentUrl { get; set; }
}
