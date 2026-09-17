namespace Core.DTOs.Response;

public class PublicStatsResponse
{
    public int TotalVehicles { get; set; }
    public int AvailableVehicles { get; set; }
    public int TotalBookings { get; set; }
    public double AverageRating { get; set; }
    public int TotalCustomers { get; set; }
}
