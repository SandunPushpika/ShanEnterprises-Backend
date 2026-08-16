namespace Core.DTOs.Response;

public class DashboardStatsResponse
{
    // Booking metrics
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int OngoingBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
    
    // User metrics
    public int TotalCustomers { get; set; }
    public int TotalDrivers { get; set; }
    public int ApprovedDrivers { get; set; }
    public int PendingDrivers { get; set; }
    public int DeactivatedDrivers { get; set; }
    
    // Vehicle metrics
    public int TotalVehicles { get; set; }
    public int AvailableVehicles { get; set; }
    public int BookedVehicles { get; set; }
    public int MaintenanceVehicles { get; set; }
    public int UnavailableVehicles { get; set; }
    
    // Revenue metrics (from payments)
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    
    // Recent bookings for dashboard table (last 10)
    public IReadOnlyCollection<BookingReadResponse> RecentBookings { get; set; } = new List<BookingReadResponse>();
    
    // Pending driver requests for dashboard widget
    public IReadOnlyCollection<DriverResponse> PendingDriverRequests { get; set; } = new List<DriverResponse>();
    
    // Chart data: bookings by status
    public IReadOnlyCollection<ChartDataPoint> BookingsByStatus { get; set; } = new List<ChartDataPoint>();
    
    // Chart data: bookings created per day for last 30 days
    public IReadOnlyCollection<TimeSeriesDataPoint> BookingsOverTime { get; set; } = new List<TimeSeriesDataPoint>();
    
    // Chart data: revenue per month for last 6 months
    public IReadOnlyCollection<TimeSeriesDataPoint> RevenueOverTime { get; set; } = new List<TimeSeriesDataPoint>();
}

public class ChartDataPoint
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public string? Color { get; set; }
}

public class TimeSeriesDataPoint
{
    public string Date { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
