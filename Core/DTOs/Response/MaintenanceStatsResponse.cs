namespace Core.DTOs.Response;

public class MaintenanceStatsResponse
{
    public string Label { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
    public int RecordCount { get; set; }
}
