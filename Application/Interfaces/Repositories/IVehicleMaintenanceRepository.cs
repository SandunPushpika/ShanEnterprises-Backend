using Core.DTOs.Request.Maintenance;
using Core.DTOs.Response;
using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IVehicleMaintenanceRepository
{
    Task AddVehicleMaintenance(VehicleMaintenance vehicleMaintenance);
    Task<(IReadOnlyCollection<VehicleMaintenance> VehicleMaintenances,int Total)> GetAllVehicleMaintenances();
    Task<VehicleMaintenance?> GetVehicleMaintenanceById(int id);
    Task UpdateVehicleMaintenance(VehicleMaintenance vehicleMaintenance);
    Task DeleteVehicleMaintenance(VehicleMaintenance vehicleMaintenance);
    Task<(IReadOnlyCollection<VehicleMaintenance> VehicleMaintenances, int Total)> SearchVehicleMaintenances(MaintenanceSearchRequest request);
    Task<IReadOnlyCollection<MaintenanceStatsResponse>> GetMaintenanceStats(string groupBy, int? year);

}