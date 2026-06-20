using Core.Enums;
using System;
using Core.Entities;

namespace Core.DTOs.Response;
public class VehicleResponse
{
    public int Id { get; set; }
    public VehicleBrand? Brand { get; set; }
    public VehicleType? Type { get; set; }
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
    public bool AirConditioned { get; set; }
    public bool HasBluetooth { get; set; }
    public bool HasGps { get; set; }
    public VehicleStatus Status { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalBookings { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<VehicleImages>? VehicleImages { get; set; }
}