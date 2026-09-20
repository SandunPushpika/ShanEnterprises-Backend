namespace Core.DTOs.Response;

public class VehicleStatsResponse
{
    public int Total { get; set; }
    public int Available { get; set; }
    public int Rented { get; set; }
    public int Maintenance { get; set; }
    public int Unavailable { get; set; }
}
