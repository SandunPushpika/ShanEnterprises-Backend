using Application.Interfaces.Repositories;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Core.DTOs.Request.Bookings;
namespace Application.Repositories;

public class BookingRepository(AppDbContext context) : IBookingRepository
{
    public async Task AddBooking(Booking booking)
    {
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
    }

    public async Task<bool> IsBooked(int vehicleId, DateTime from, DateTime to)
    {
        var isBooked = await context.Bookings
            .AnyAsync(b =>
                b.VehicleId == vehicleId &&
                b.BookingStatus != BookingStatus.CANCELLED &&
                b.PickupDatetime < to &&
                b.ReturnDatetime > from
            );
        return isBooked;
    }

    public Task<Booking?> GetBookingById(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateBooking(Booking booking)
    {
        throw new NotImplementedException();
    }

    public async Task<(IReadOnlyCollection<Booking> Bookings, int Total)> GetAllBookings(BookingSearchRequest request)
    {
        var query = context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Vehicle)
            .AsQueryable();

        if (request.VehicleId.HasValue && request.VehicleId != 0)
        {
            query = query.Where(b => b.VehicleId == request.VehicleId.Value);
        }
        
        if (request.CustomerId.HasValue && request.CustomerId.Value != 0L)
        {
            query = query.Where(b => b.CustomerId == request.CustomerId.Value);
        }

        if (request.DriverId.HasValue && request.DriverId != 0)
        {
            query = query.Where(b => b.DriverId == request.DriverId.Value);
        }

        if (request.WithDriver.HasValue)
        {
            query = query.Where(b => b.WithDriver == request.WithDriver.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.BookingStatus == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.BookingReference))
        {
            query = query.Where(b => b.BookingReference.Contains(request.BookingReference));
        }

        if (request.PickupFrom.HasValue)
        {
            query = query.Where(b => b.PickupDatetime >= request.PickupFrom.Value);
        }
    
        if (request.PickupTo.HasValue)
        {
            query = query.Where(b => b.PickupDatetime <= request.PickupTo.Value);
        }

        if (request.ReturnFrom.HasValue)
        {
            query = query.Where(b => b.ReturnDatetime >= request.ReturnFrom.Value);
        }
        if (request.ReturnTo.HasValue)
        {
            query = query.Where(b => b.ReturnDatetime <= request.ReturnTo.Value);
        }

        var total = await query.CountAsync();
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize   = request.PageSize   < 1 ? 10 : request.PageSize;
        var bookings = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (bookings, total);
    }

    public Task DeleteBooking(Booking booking)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyCollection<BookedDateRangeResponse>> GetBookedDatesByVehicleId(int vehicleId)
    {
        var activeStatuses = new[]
        {
            BookingStatus.PENDING,
            BookingStatus.CONFIRMED,
            BookingStatus.ONGOING
        };

        var ranges = await context.Bookings
            .Where(b => b.VehicleId == vehicleId && activeStatuses.Contains(b.BookingStatus))
            .OrderBy(b => b.PickupDatetime)
            .Select(b => new BookedDateRangeResponse
            {
                StartDate = b.PickupDatetime.ToString("yyyy-MM-dd"),
                EndDate   = b.ReturnDatetime.ToString("yyyy-MM-dd")
            })
            .ToListAsync();

        return ranges.AsReadOnly();
    }
}