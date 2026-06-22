using Application.Interfaces.Repositories;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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

    public Task<IReadOnlyCollection<Booking>> GetAllBookings()
    {
        throw new NotImplementedException();
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