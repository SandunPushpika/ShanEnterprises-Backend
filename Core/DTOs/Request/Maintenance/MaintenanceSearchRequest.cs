namespace Core.DTOs.Request.Maintenance;

public class MaintenanceSearchRequest
{
    public int? VehicleId { get; set; }         
    public int? Month { get; set; }               
    public int? Year { get; set; }                
    public string? Status { get; set; }           
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
