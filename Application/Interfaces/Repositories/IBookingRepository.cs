using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;
using Core.Entities;
namespace Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<Booking> AddBooking(Booking booking);
    Task<Booking?> GetBookingById(int id);
    Task UpdateBooking(Booking booking);
    Task<(IReadOnlyCollection<Booking> Bookings,int Total)>  GetAllBookings(BookingSearchRequest request);
    Task DeleteBooking(Booking booking);
    Task<bool> IsBooked(int vehicleId, DateTime from, DateTime to);
    Task<IReadOnlyCollection<BookedDateRangeResponse>> GetBookedDatesByVehicleId(int vehicleId);
    Task SaveAsync();
}