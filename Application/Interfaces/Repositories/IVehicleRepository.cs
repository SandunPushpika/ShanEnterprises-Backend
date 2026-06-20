using Core.Entities;
using Core.DTOs.Request;
using System.Collections.Generic;

namespace Application.Interfaces.Repositories;

public interface IVehicleRepository
{
    Task<int>  AddVehicle(Vehicle vehicle);
    Task<Vehicle?> GetVehicleById(int id, bool includeTypes = false, bool includeImages = false, bool includeBrands = false);
    Task UpdateVehicle(Vehicle vehicle);
    Task<(IReadOnlyCollection<Vehicle> Vehicles, int Total)> SearchVehicles(VehicleSearchRequest request);
    Task<IReadOnlyCollection<VehicleBrand>> GetAllVehicleBrands();
    Task<IReadOnlyCollection<VehicleType>> GetAllVehicleTypes();
    Task AddVehicleImagesAsync(IEnumerable<VehicleImages> vehicleImages);
    Task DeleteVehicleImagesByVehicleAsync(int vehicleId);
    Task<List<VehicleImages>> GetVehicleImagesByVehicleIdAsync(int vehicleId);
}