namespace Core.DTOs.Response;

public class MaintenanceReadResponse
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string? VehicleRegistrationNumber { get; set; }
    public string? VehicleName { get; set; }   
    public DateOnly MaintenanceStart { get; set; }
    public DateOnly MaintenanceEnd { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
