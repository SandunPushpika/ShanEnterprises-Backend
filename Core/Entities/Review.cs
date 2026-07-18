using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Review
{
    [Key]
    public int Id { get; set; }

    public int BookingId { get; set; }

    public long CustomerId { get; set; }

    public int? VehicleId { get; set; }

    public int? DriverId { get; set; }

    public int? VehicleRating { get; set; }

    public int? DriverRating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Booking Booking { get; set; } = null!;

    public virtual User Customer { get; set; } = null!;

    public virtual Vehicle? Vehicle { get; set; }   
    
    // public virtual Driver? Driver { get; set; }
}