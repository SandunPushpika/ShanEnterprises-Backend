using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("driver_booking_cancellations")]
public class DriverBookingCancellation
{
    [Key]
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int DriverId { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CancelledAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual Booking Booking { get; set; } = null!;
    public virtual Driver Driver { get; set; } = null!;
}
