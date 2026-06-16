namespace Core.DTOs.Response;

public class BookingReadResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int VehicleId { get; set; }
    public string VehicleModel { get; set; } = string.Empty;
    public int? DriverId { get; set; }
    public string? DriverName { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string PickupLocation { get; set; } = string.Empty;
    public string? DropoffLocation { get; set; }
    public DateTime PickupDateTime { get; set; }
    public DateTime ReturnDateTime { get; set; }
    public int? RentalDays { get; set; }
    public decimal? EstimatedDistanceKm { get; set; }
    public bool WithDriver { get; set; }
    public decimal BaseRentalCost { get; set; }
    public decimal DriverFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? SpecialNotes { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public string? CancelledReason { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}