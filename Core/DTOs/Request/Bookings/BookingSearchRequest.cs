using Core.Enums;

namespace Core.DTOs.Request.Bookings;

public class BookingSearchRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? VehicleId { get; set; }
    public long? CustomerId { get; set; }
    public int? DriverId { get; set; }
    public bool? WithDriver { get; set; }
    public BookingStatus? Status { get; set; }
    public string? BookingReference { get; set; }
    public DateTime? PickupFrom { get; set; }
    public DateTime? PickupTo { get; set; }
    public DateTime? ReturnFrom { get; set; }
    public DateTime? ReturnTo { get; set; }
    public string? Search { get; set; }
}

