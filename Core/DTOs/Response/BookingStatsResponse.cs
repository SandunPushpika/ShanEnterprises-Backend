namespace Core.DTOs.Response;

public class BookingStatsResponse
{
    public int Total { get; set; }
    public int Confirmed { get; set; }
    public int Completed { get; set; }
    public int Pending { get; set; }
    public int Cancelled { get; set; }
}
