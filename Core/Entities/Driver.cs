using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Entities;

[Table("drivers")]
public class Driver
{
    [Key]
    public int Id { get; set; }

    [Required]
    public long UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string LicenseNumber { get; set; } = null!;

    [Required]
    public DateOnly LicenseExpiryDate { get; set; }

    public int YearsOfExperience { get; set; } = 0;

    [Required]
    public DriverStatus DriverStatus { get; set; } = DriverStatus.PENDING;

    public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.AVAILABLE;

    public string? LicenseDocumentUrl { get; set; }

    [Column(TypeName = "decimal(3,2)")]
    public decimal AverageRating { get; set; } = 0.00m;

    public int CompletedRides { get; set; } = 0;

    public long? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;

    public virtual User? ApprovedByUser { get; set; }
}
