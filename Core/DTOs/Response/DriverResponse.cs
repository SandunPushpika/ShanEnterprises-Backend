using Core.Enums;

namespace Core.DTOs.Response;

public class DriverResponse
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateOnly LicenseExpiryDate { get; set; }
    public int YearsOfExperience { get; set; }
    public DriverStatus DriverStatus { get; set; }
    public AvailabilityStatus Availability { get; set; }
    public string? LicenseDocumentUrl { get; set; }
    public decimal AverageRating { get; set; }
    public int CompletedRides { get; set; }
    public long? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
