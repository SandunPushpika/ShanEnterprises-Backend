using Core.DTOs.Request;
using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IBookingService
{
    Task<string> AddBooking(BookingCreateRequest request);  
    Task UpdateBooking(int id, BookingUpdateRequest request);
    Task<BookingReadResponse> GetBookingById(int id);
    Task<SearchResponse<BookingReadResponse>> GetAllBookings(BookingSearchRequest request);
    Task UpdateBookingStatus(int id, BookingStatusUpdatRequest request);
    Task DeleteBooking(int id);
    Task<IReadOnlyCollection<BookedDateRangeResponse>> GetBookedDatesByVehicleId(int vehicleId);
    Task<SearchResponse<BookingReadResponse>> GetAllBookingsForUser(BookingSearchRequest request);
    Task<bool> VerifyBooking(string sessionId);
}