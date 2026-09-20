namespace Core.DTOs.Response;

public class CustomerStatsResponse
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
    public int Verified { get; set; }
}
