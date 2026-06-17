namespace Core.DTOs.Request.Bookings;

public class BookingCreateRequest
{
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    public int? DriverId { get; set; }
    public DateTime PickupDateTime { get; set; }
    public DateTime ReturnDateTime { get; set; }
    public bool WithDriver { get; set; }
    public decimal BaseRentalCost { get; set; }
    public decimal DriverFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? SpecialNotes { get; set; }
}