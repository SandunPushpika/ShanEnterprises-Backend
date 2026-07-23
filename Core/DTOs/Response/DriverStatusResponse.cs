using Core.Enums;

namespace Core.DTOs.Response;

public class DriverStatusResponse
{
    public int? DriverId { get; set; }
    public DriverStatus? Status { get; set; }
    public bool HasRequest { get; set; }
    public string? Message { get; set; }
}
