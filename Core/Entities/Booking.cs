using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Entities;

public class Booking
{
    [Key] 
    public int Id { get; set; }

    public long CustomerId { get; set; }
    public int VehicleId { get; set; }
    public int? DriverId { get; set; }

    [Required] 
    [MaxLength(50)] 
    public string BookingReference { get; set; } = string.Empty;

    [Required]
    public string PickupLocation { get; set; } = string.Empty;
    public string? DropoffLocation { get; set; }

    [Required] 
    public DateTime PickupDatetime { get; set; }

    [Required]
    public DateTime ReturnDatetime { get; set; }
    public int? RentalDays { get; set; }

    [Column(TypeName = "decimal(10,2)")] 
    public decimal? EstimatedDistanceKm { get; set; }
    public bool WithDriver { get; set; } = false;

    [Column(TypeName = "decimal(10,2)")]
    public decimal BaseRentalCost { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal DriverFee { get; set; } = 0.00m;

    [Column(TypeName = "decimal(10,2)")]
    public decimal TaxAmount { get; set; } = 0.00m;

    [Column(TypeName = "decimal(10,2)")]
    public decimal DiscountAmount { get; set; } = 0.00m;

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    public string? SpecialNotes { get; set; }

    public BookingStatus BookingStatus { get; set; }

    public string? CancelledReason { get; set; }
    public DateTime? CancelledAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User Customer { get; set; } = null!;
    public virtual Vehicle Vehicle { get; set; } = null!;
    public virtual Driver? Driver { get; set; }
    public virtual ICollection<DriverBookingCancellation> DriverCancellations { get; set; } = new List<DriverBookingCancellation>();
}