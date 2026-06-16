using Application.Interfaces.Repositories;
using Core.Entities;
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
}