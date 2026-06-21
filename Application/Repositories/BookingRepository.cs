using Application.Interfaces.Repositories;
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
}