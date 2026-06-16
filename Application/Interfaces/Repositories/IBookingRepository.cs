using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task AddBooking(Booking booking);
    Task<Booking?> GetBookingById(int id);
    Task UpdateBooking(Booking booking);
    Task<IReadOnlyCollection<Booking>> GetAllBookings();
    Task DeleteBooking(Booking booking);
}