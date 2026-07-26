namespace Core.DTOs.Response;

public class DriverTripCancelResponse
{
    public bool Reassigned { get; set; }
    public int? NewDriverId { get; set; }
    public string? NewDriverName { get; set; }
    public string Message { get; set; } = string.Empty;
}
