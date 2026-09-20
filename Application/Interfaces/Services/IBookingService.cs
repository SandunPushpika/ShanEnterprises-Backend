using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IBookingService
{
    Task<string> AddBooking(BookingCreateRequest request);  
    Task UpdateBooking(int id, BookingUpdateRequest request);
    Task<BookingReadResponse> GetBookingById(int id);
    Task<SearchResponse<BookingReadResponse>> GetAllBookings(BookingSearchRequest request);
    Task DeleteBooking(int id);
    Task ChangeBookingDriverAsync(int bookingId, int? driverId);
    Task AutoAssignDriverAsync(int bookingId);
    Task<IReadOnlyCollection<BookedDateRangeResponse>> GetBookedDatesByVehicleId(int vehicleId);
    Task<SearchResponse<BookingReadResponse>> GetAllBookingsForUser(BookingSearchRequest request);
    Task<bool> VerifyBooking(string sessionId);
    Task CompleteBooking(int bookingId);
    Task AssignDriverToBooking(int bookingId, AssignDriverRequest request);
    Task<BookingStatsResponse> GetBookingStatsAsync();
}