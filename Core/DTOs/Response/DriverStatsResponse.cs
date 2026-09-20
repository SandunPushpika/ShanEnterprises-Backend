namespace Core.DTOs.Response;

public class DriverStatsResponse
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Blocked { get; set; }
}
