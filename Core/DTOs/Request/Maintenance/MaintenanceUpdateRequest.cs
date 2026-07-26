using Core.Enums;

namespace Core.DTOs.Request.Maintenance;

public class MaintenanceUpdateRequest
{
    public DateOnly MaintenanceStart { get; set; }
    public DateOnly MaintenanceEnd { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.UNDER_MAINTENANCE;
}
