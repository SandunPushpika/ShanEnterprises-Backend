using Application.Interfaces.Repositories;
using AutoMapper;
using Core.DTOs.Response;
using Core.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DashboardRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DashboardStatsResponse> GetDashboardStatsAsync()
    {
        var response = new DashboardStatsResponse();

        // Bookings
        response.TotalBookings = await _context.Bookings.CountAsync();
        response.PendingBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == BookingStatus.PENDING);
        response.ConfirmedBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == BookingStatus.CONFIRMED);
        response.OngoingBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == BookingStatus.ONGOING);
        response.CompletedBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == BookingStatus.COMPLETED);
        response.CancelledBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == BookingStatus.CANCELLED);

        // Users & Drivers
        response.TotalCustomers = await _context.Users.CountAsync(u => u.Role == UserRole.CUSTOMER);
        response.TotalDrivers = await _context.Drivers.CountAsync();
        response.ApprovedDrivers = await _context.Drivers.CountAsync(d => d.DriverStatus == DriverStatus.APPROVED);
        response.PendingDrivers = await _context.Drivers.CountAsync(d => d.DriverStatus == DriverStatus.PENDING);
        response.DeactivatedDrivers = await _context.Drivers.CountAsync(d => d.DriverStatus == DriverStatus.DEACTIVATED);

        // Vehicles
        response.TotalVehicles = await _context.Vehicles.CountAsync();
        response.AvailableVehicles = await _context.Vehicles.CountAsync(v => v.Status == VehicleStatus.AVAILABLE);
        response.BookedVehicles = await _context.Vehicles.CountAsync(v => v.Status == VehicleStatus.BOOKED);
        response.MaintenanceVehicles = await _context.Vehicles.CountAsync(v => v.Status == VehicleStatus.MAINTENANCE);
        response.UnavailableVehicles = await _context.Vehicles.CountAsync(v => v.Status == VehicleStatus.UNAVAILABLE);

        // Revenue
        response.TotalRevenue = await _context.Payments
            .Where(p => p.PaymentStatus == PaymentStatus.COMPLETED)
            .SumAsync(p => p.Amount);
            
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        response.MonthlyRevenue = await _context.Payments
            .Where(p => p.PaymentStatus == PaymentStatus.COMPLETED && p.CreatedAt >= startOfMonth)
            .SumAsync(p => p.Amount);

        // Recent Bookings
        var recentBookings = await _context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Vehicle)
            .OrderByDescending(b => b.CreatedAt)
            .Take(10)
            .ToListAsync();
        response.RecentBookings = _mapper.Map<IReadOnlyCollection<BookingReadResponse>>(recentBookings);

        // Pending Driver Requests
        var pendingDrivers = await _context.Drivers
            .Include(d => d.User)
            .Where(d => d.DriverStatus == DriverStatus.PENDING)
            .OrderByDescending(d => d.CreatedAt)
            .Take(5)
            .ToListAsync();
        response.PendingDriverRequests = _mapper.Map<IReadOnlyCollection<DriverResponse>>(pendingDrivers);

        // Charts
        response.BookingsByStatus = new List<ChartDataPoint>
        {
            new ChartDataPoint { Label = "Pending", Value = response.PendingBookings, Color = "#f59e0b" },
            new ChartDataPoint { Label = "Confirmed", Value = response.ConfirmedBookings, Color = "#3b82f6" },
            new ChartDataPoint { Label = "Ongoing", Value = response.OngoingBookings, Color = "#8b5cf6" },
            new ChartDataPoint { Label = "Completed", Value = response.CompletedBookings, Color = "#10b981" },
            new ChartDataPoint { Label = "Cancelled", Value = response.CancelledBookings, Color = "#ef4444" }
        };

        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30).Date;
        
        var bookingsOverTime = await _context.Bookings
            .Where(b => b.CreatedAt >= thirtyDaysAgo)
            .GroupBy(b => b.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(g => g.Date)
            .ToListAsync();

        response.BookingsOverTime = bookingsOverTime.Select(b => new TimeSeriesDataPoint
        {
            Date = b.Date.ToString("yyyy-MM-dd"),
            Value = b.Count
        }).ToList();

        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        sixMonthsAgo = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var revenueOverTime = await _context.Payments
            .Where(p => p.PaymentStatus == PaymentStatus.COMPLETED && p.CreatedAt >= sixMonthsAgo)
            .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(p => p.Amount) })
            .ToListAsync();

        response.RevenueOverTime = revenueOverTime
            .OrderBy(r => r.Year).ThenBy(r => r.Month)
            .Select(r => new TimeSeriesDataPoint
            {
                Date = $"{r.Year}-{r.Month:D2}",
                Value = r.Total
            }).ToList();

        return response;
    }

    public async Task<PublicStatsResponse> GetPublicStatsAsync()
    {
        // Fast count queries 
        var totalVehicles = await _context.Vehicles.CountAsync();
        var availableVehicles = await _context.Vehicles.CountAsync(v => v.Status == VehicleStatus.AVAILABLE);
        var totalBookings = await _context.Bookings.CountAsync();
        var totalCustomers = await _context.Users.CountAsync(u => u.Role == UserRole.CUSTOMER);

        // Calculate real average rating 
        var hasReviews = await _context.Reviews.AnyAsync(r => r.VehicleRating.HasValue);
        var averageRating = hasReviews
            ? await _context.Reviews
                .Where(r => r.VehicleRating.HasValue)
                .AverageAsync(r => (double)r.VehicleRating!.Value)
            : 4.9;

        return new PublicStatsResponse
        {
            TotalVehicles = totalVehicles,
            AvailableVehicles = availableVehicles,
            TotalBookings = totalBookings,
            AverageRating = Math.Round(averageRating, 1),
            TotalCustomers = totalCustomers
        };
    }

}
