using Core.Enums;

namespace Core.Entities;

public class Vehicle
{
    public int Id { get; set; }
    
    public int? BrandId { get; set; }
    public int? TypeId { get; set; }

    public virtual VehicleBrand? Brand { get; set; }
    public virtual VehicleType? Type { get; set; }
    
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

    public VehicleStatus Status { get; set; } = VehicleStatus.AVAILABLE;
    
    public decimal AverageRating { get; set; } = 0.00m;
    public int TotalBookings { get; set; } = 0;

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    
    public virtual List<VehicleImages>? VehicleImages { get; set; }
}