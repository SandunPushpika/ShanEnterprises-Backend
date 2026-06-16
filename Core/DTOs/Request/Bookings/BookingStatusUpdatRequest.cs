namespace Core.DTOs.Request.Bookings;

public class BookingStatusUpdatRequest
{
    public string BookingStatus { get; set; } = string.Empty;

    public string? CancelledReason { get; set; }
}