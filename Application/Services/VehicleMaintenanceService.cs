using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Maintenance;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.DTOs.Response;

namespace Application.Services;

public class VehicleMaintenanceService : IVehicleMaintenanceService
{
    private readonly IVehicleMaintenanceRepository _vehicleMaintenanceRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public VehicleMaintenanceService(
        IVehicleMaintenanceRepository vehicleMaintenanceRepository,
        IVehicleRepository vehicleRepository,
        IMapper mapper)
    {
        
        this._vehicleMaintenanceRepository = vehicleMaintenanceRepository;
        this._vehicleRepository = vehicleRepository;
        this._mapper = mapper;
        
    }

    public async Task AddVehicleMaintenance(MaintenanceCreateRequest request)
    {
        var vehicle = await _vehicleRepository.GetVehicleById(request.VehicleId);

        if (vehicle == null)
            throw new NotFoundException($"Vehicle with id {request.VehicleId} not found");

        var vehicleMaintenance = _mapper.Map<VehicleMaintenance>(request);
            vehicleMaintenance.CreatedAt = DateTime.UtcNow;
            vehicleMaintenance.UpdatedAt = DateTime.UtcNow;

        await _vehicleMaintenanceRepository.AddVehicleMaintenance(vehicleMaintenance);

        vehicle.Status = request.Status == MaintenanceStatus.COMPLETED 
            ? VehicleStatus.AVAILABLE 
            : VehicleStatus.MAINTENANCE;
            
        vehicle.CreatedAt = DateTime.SpecifyKind(vehicle.CreatedAt, DateTimeKind.Utc);
        vehicle.UpdatedAt = DateTime.UtcNow;
        
        await _vehicleRepository.UpdateVehicle(vehicle);
    }

    public async Task<SearchResponse<MaintenanceReadResponse>> GetAllVehicleMaintenances()
    {
        
        var (maintenances, total) = await _vehicleMaintenanceRepository.GetAllVehicleMaintenances();
        var response = new SearchResponse<MaintenanceReadResponse>
    {
        Data = _mapper.Map<IReadOnlyCollection<MaintenanceReadResponse>>(maintenances),
        Total = total
    };
        return response;
    }

    public async Task<MaintenanceReadResponse> GetVehicleMaintenanceById(int id)
    {
        var vehicleMaintenance = await _vehicleMaintenanceRepository.GetVehicleMaintenanceById(id);
        if (vehicleMaintenance == null)
            throw new NotFoundException($"Maintenance record with id {id} not found");

        return _mapper.Map<MaintenanceReadResponse>(vehicleMaintenance);
    }

    public async Task UpdateVehicleMaintenance(int id, MaintenanceUpdateRequest request)
    {
        
        var vehicleMaintenance = await _vehicleMaintenanceRepository.GetVehicleMaintenanceById(id);
        if (vehicleMaintenance == null)
            throw new NotFoundException($"Maintenance record with id {id} not found");

        _mapper.Map(request, vehicleMaintenance);
        
        vehicleMaintenance.CreatedAt = DateTime.SpecifyKind(vehicleMaintenance.CreatedAt, DateTimeKind.Utc);
        vehicleMaintenance.UpdatedAt = DateTime.UtcNow;

        await _vehicleMaintenanceRepository.UpdateVehicleMaintenance(vehicleMaintenance);

        var vehicle = await _vehicleRepository.GetVehicleById(vehicleMaintenance.VehicleId);
        if (vehicle != null)
        {
        
             vehicle.Status = request.Status == MaintenanceStatus.COMPLETED 
                ? VehicleStatus.AVAILABLE 
                : VehicleStatus.MAINTENANCE;
            
            vehicle.CreatedAt = DateTime.SpecifyKind(vehicle.CreatedAt, DateTimeKind.Utc);
            vehicle.UpdatedAt = DateTime.UtcNow;
            
            await _vehicleRepository.UpdateVehicle(vehicle);
        }
    }

    public async Task DeleteVehicleMaintenance(int id)
    {
        var vehicleMaintenance = await _vehicleMaintenanceRepository.GetVehicleMaintenanceById(id);
        if (vehicleMaintenance == null)
            throw new NotFoundException($"Maintenance record with id {id} not found");

        await _vehicleMaintenanceRepository.DeleteVehicleMaintenance(vehicleMaintenance);

        var vehicle = await _vehicleRepository.GetVehicleById(vehicleMaintenance.VehicleId);
        if (vehicle != null)
        {
            vehicle.Status = VehicleStatus.AVAILABLE;
            vehicle.CreatedAt = DateTime.SpecifyKind(vehicle.CreatedAt, DateTimeKind.Utc);
            vehicle.UpdatedAt = DateTime.UtcNow;
            await _vehicleRepository.UpdateVehicle(vehicle);
        }
    }

    public async Task<SearchResponse<MaintenanceReadResponse>> SearchVehicleMaintenances(MaintenanceSearchRequest request)
    {
        var (maintenances, total) = await _vehicleMaintenanceRepository.SearchVehicleMaintenances(request);
        return new SearchResponse<MaintenanceReadResponse> 
        {
            Data = _mapper.Map<IReadOnlyCollection<MaintenanceReadResponse>>(maintenances),
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IReadOnlyCollection<MaintenanceStatsResponse>> GetMaintenanceStats(string groupBy, int? year)
    {
        return await _vehicleMaintenanceRepository.GetMaintenanceStats(groupBy,year);
    }
}