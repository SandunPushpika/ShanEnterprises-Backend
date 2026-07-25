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
    Task<IReadOnlyCollection<DriverResponse>> GetAvailableDriversAsync();
    Task<DriverResponse> GetDriverByIdAsync(int driverId);
}
