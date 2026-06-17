using Core.Enums;

namespace Core.DTOs.Request;

public class VehicleUpdateRequest
{
    public int? BrandId { get; set; }
    public int? TypeId { get; set; }
    
    public string Model { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;

    public int ManufactureYear { get; set; }
    public string? Color { get; set; }
    
    public TransmissionType? Transmission { get; set; }
    public FuelType? Fuel { get; set; }
    
    public int? SeatCapacity { get; set; }
    public int? LuggageCapacity { get; set; }
    
    public decimal DailyRentalPrice { get; set; }
    public decimal? PricePerKm { get; set; }
    
    public string? Description { get; set; }
    public string? MainImageUrl { get; set; }

    public bool AirConditioned { get; set; } = true;
    public bool HasBluetooth { get; set; } = false;
    public bool HasGps { get; set; } = false;
    
    public IReadOnlyList<string> ImageUrls { get; set; } = new List<string>();
}