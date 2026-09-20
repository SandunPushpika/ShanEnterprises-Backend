using Core.DTOs.Request.Driver;
using Core.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IDriverService
{
    Task SubmitDriverRequestAsync(DriverCreateRequest createRequest);
    Task ApproveDriverAsync(int driverId);
    Task RejectDriverAsync(int driverId);
    Task<SearchResponse<DriverResponse>> GetDriversByStatusAsync(DriverSearchRequest request);
    Task<DriverStatusResponse> GetMyDriverStatusAsync();
    Task<IReadOnlyCollection<DriverResponse>> GetAvailableDriversAsync(AvailableDriverRequest request);
    Task<DriverResponse> GetDriverByIdAsync(int driverId);
    Task<SearchResponse<BookingReadResponse>> GetMyTripsAsync(int pageNumber = 1, int pageSize = 10);
    Task<SearchResponse<BookingReadResponse>> GetDriverTripsAsync(int driverId, int pageNumber = 1, int pageSize = 10);
    Task<DriverTripCancelResponse> CancelTripAssignmentAsync(int bookingId);
    Task<DriverStatsResponse> GetDriverStatsAsync();
}

