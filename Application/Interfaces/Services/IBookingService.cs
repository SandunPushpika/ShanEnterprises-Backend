using Core.DTOs.Request;
using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IBookingService
{
    Task AddBooking(BookingCreateRequest request);  
    Task UpdateBooking(int id, BookingUpdateRequest request);
    Task<BookingReadResponse> GetBookingById(int id);
    Task<SearchResponse<BookingReadResponse>> GetAllBookings(BookingSearchRequest request);
    Task UpdateBookingStatus(int id, BookingStatusUpdatRequest request);
    Task DeleteBooking(int id);
}