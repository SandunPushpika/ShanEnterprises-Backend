using Core.DTOs.Request.Maintenance;
using Core.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IVehicleMaintenanceService
{ 
    Task AddVehicleMaintenance(MaintenanceCreateRequest request);
    Task<MaintenanceReadResponse> GetVehicleMaintenanceById(int id); 
    Task<SearchResponse<MaintenanceReadResponse>> GetAllVehicleMaintenances();
    Task UpdateVehicleMaintenance(int id, MaintenanceUpdateRequest request);
    Task DeleteVehicleMaintenance(int id); 
    Task<SearchResponse<MaintenanceReadResponse>> SearchVehicleMaintenances(MaintenanceSearchRequest request);
    Task<IReadOnlyCollection<MaintenanceStatsResponse>> GetMaintenanceStats(string groupBy, int? year);
}