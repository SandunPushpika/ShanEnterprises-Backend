using Application.Interfaces.Repositories;
using Core.DTOs.Request.Driver;
using Core.Entities;
using Core.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories;

public class DriverRepository(AppDbContext context) : IDriverRepository
{
    public async Task<Driver> AddDriverAsync(Driver driver)
    {
        var result = await context.Drivers.AddAsync(driver);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task UpdateDriverAsync(Driver driver)
    {
        context.Drivers.Update(driver);
        await context.SaveChangesAsync();
    }

    public async Task<Driver?> GetDriverByIdAsync(int id)
    {
        return await context.Drivers
            .Include(d => d.User)
            .Include(d => d.ApprovedByUser)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Driver?> GetDriverByUserIdAsync(long userId)
    {
        return await context.Drivers
            .Include(d => d.User)
            .Include(d => d.ApprovedByUser)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<bool> HasActiveRequestAsync(long userId)
    {
        return await context.Drivers.AnyAsync(d =>
            d.UserId == userId &&
            d.DriverStatus != DriverStatus.REJECTED &&
            d.DriverStatus != DriverStatus.DEACTIVATED);
    }

    public async Task<(IReadOnlyCollection<Driver> Drivers, int Total)> GetDriversByStatusAsync(DriverSearchRequest request)
    {
        var query = context.Drivers
            .Include(d => d.User)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(d => d.DriverStatus == request.Status.Value);

        var total = await query.CountAsync();
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize   = request.PageSize   < 1 ? 10 : request.PageSize;

        var drivers = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (drivers, total);
    }

    public async Task<IReadOnlyCollection<Driver>> GetAvailableDriversAsync(
        DateTime pickupDatetime, DateTime returnDatetime)
    {
        var blockingStatuses = new[]
        {
            BookingStatus.PENDING,
            BookingStatus.CONFIRMED,
            BookingStatus.ONGOING
        };
        pickupDatetime = DateTime.SpecifyKind(pickupDatetime, DateTimeKind.Utc);
        returnDatetime = DateTime.SpecifyKind(returnDatetime, DateTimeKind.Utc);

        var busyDriverIds = context.Bookings
            .Where(b =>
                b.DriverId.HasValue &&
                blockingStatuses.Contains(b.BookingStatus) &&
                b.PickupDatetime  < returnDatetime &&
                b.ReturnDatetime  > pickupDatetime)
            .Select(b => b.DriverId!.Value);

        return await context.Drivers
            .Include(d => d.User)
            .Where(d =>
                d.DriverStatus == DriverStatus.APPROVED &&
                d.Availability == AvailabilityStatus.AVAILABLE &&
                !busyDriverIds.Contains(d.Id))
            .OrderBy(d => d.User.FirstName)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Booking>> GetDriverTripsAsync(int driverId)
    {
        return await context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Vehicle)
            .Where(b => b.DriverId == driverId)
            .OrderByDescending(b => b.PickupDatetime)
            .ToListAsync();
    }

    public async Task<(IReadOnlyCollection<Booking> Trips, int Total)> GetDriverTripsPaginatedAsync(int driverId, int pageNumber, int pageSize)
    {
        var query = context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Vehicle)
            .Where(b => b.DriverId == driverId)
            .OrderByDescending(b => b.PickupDatetime);
        
        var total = await query.CountAsync();
        var pn = pageNumber < 1 ? 1 : pageNumber;
        var ps = pageSize < 1 ? 10 : pageSize;
        var trips = await query.Skip((pn - 1) * ps).Take(ps).ToListAsync();
        return (trips, total);
    }

    public async Task AddDriverBookingCancellationAsync(DriverBookingCancellation cancellation)
    {
        await context.DriverBookingCancellations.AddAsync(cancellation);
        await context.SaveChangesAsync();
    }
}
