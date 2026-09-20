using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;

namespace Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<Booking> AddBooking(Booking booking);
    Task<Booking?> GetBookingById(int id);
    Task<bool> HasActiveBookingsForDriverAsync(int driverId, BookingStatus[] statuses);
    Task<Booking?> GetBookingByIdForCustomerAsync(int bookingId, long customerId);
    Task UpdateBooking(Booking booking);
    Task<(IReadOnlyCollection<Booking> Bookings,int Total)>  GetAllBookings(BookingSearchRequest request);
    Task DeleteBooking(Booking booking);
    Task<bool> IsBooked(int vehicleId, DateTime from, DateTime to);
    Task<IReadOnlyCollection<BookedDateRangeResponse>> GetBookedDatesByVehicleId(int vehicleId);
    Task SaveAsync();
    Task<Booking> GetBookingByVehicleIdAndUserId(int vehicleId, int userId,
        BookingStatus status = BookingStatus.COMPLETED);
    Task<BookingStatsResponse> GetBookingStatsAsync();
}