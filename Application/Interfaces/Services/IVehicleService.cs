using Core.DTOs.Request;
using Core.DTOs.Response;
using Core.Entities;

namespace Application.Interfaces.Services;

public interface IVehicleService
{
    Task AddVehicle(VehicleCreateRequest request);
    Task UpdateVehicle(int id, VehicleUpdateRequest request);
    Task<SearchResponse<VehicleResponse>> SearchVehicles(VehicleSearchRequest request);
    Task<IReadOnlyCollection<VehicleBrand>> GetAllBrands();
    Task<IReadOnlyCollection<VehicleType>> GetAllVehicleTypes();
    Task DeleteVehicle(int id);
    Task<List<VehicleImages>> GetVehicleImagesByVehicleIdAsync(int vehicleId);
    Task<VehicleResponse> GetVehicleById(int vehicleId);
    Task SetVehicleAvailability(int id, bool makeUnavailable);
}