using Core.DTOs.Request;
using Core.DTOs.Response;
namespace Application.Interfaces.Services;

public interface IVehicleService
{
    Task AddVehicle(VehicleCreateRequest request);
    Task UpdateVehicle(int id, VehicleUpdateRequest request);
    Task<SearchResponse<VehicleResponse>> SearchVehicles(VehicleSearchRequest request);
}