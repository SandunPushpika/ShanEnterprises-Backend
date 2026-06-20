using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request;
using Core.Entities;
using Core.Exceptions;
using Core.DTOs.Response;
using Core.Enums;

namespace Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _repository;
    private readonly IMapper _mapper;
    
    public VehicleService(IVehicleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task AddVehicle(VehicleCreateRequest request)
    {
        var vehicle = _mapper.Map<Vehicle>(request);
        vehicle.CreatedAt = DateTime.UtcNow;
        vehicle.UpdatedAt = DateTime.UtcNow;
        
        var vehicleId = await _repository.AddVehicle(vehicle);
        
        if(request.ImageUrls == null)
            return;

        await AddVehicleImages(request.ImageUrls, vehicleId);
    }

    public async Task UpdateVehicle(int id, VehicleUpdateRequest request)
    {
        var vehicle = await _repository.GetVehicleById(id);

        if (vehicle == null)
            throw new NotFoundException($"Vehicle with id {id} not found");

        _mapper.Map(request, vehicle);

        vehicle.UpdatedAt = DateTime.UtcNow;
        vehicle.CreatedAt = DateTime.SpecifyKind(vehicle.CreatedAt, DateTimeKind.Utc);

        await _repository.UpdateVehicle(vehicle);
        
        if(request.ImageUrls == null)
            return;
        
        await AddVehicleImages(request.ImageUrls, vehicle.Id);
    }
    
    public async Task<SearchResponse<VehicleResponse>> SearchVehicles(VehicleSearchRequest request)
    {
        var (vehicles, total) = await _repository.SearchVehicles(request);
        var vehicleResponses = _mapper.Map<IReadOnlyCollection<VehicleResponse>>(vehicles);
        return new SearchResponse<VehicleResponse>
        {
            Data = vehicleResponses,
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IReadOnlyCollection<VehicleBrand>> GetAllBrands()
    {
        return await _repository.GetAllVehicleBrands();
    }

    public async Task<IReadOnlyCollection<VehicleType>> GetAllVehicleTypes()
    {
        return await _repository.GetAllVehicleTypes();
    }

    public async Task DeleteVehicle(int id)
    {
        var vehicle = await _repository.GetVehicleById(id);
        if(vehicle == null || vehicle.Status == VehicleStatus.UNAVAILABLE)
            throw new NotFoundException($"Vehicle with id {id} not found");

        vehicle.Status = VehicleStatus.UNAVAILABLE;
        vehicle.UpdatedAt = DateTime.UtcNow;
        vehicle.CreatedAt = DateTime.SpecifyKind(vehicle.CreatedAt, DateTimeKind.Utc);
        
        await _repository.UpdateVehicle(vehicle);
    }

    public async Task<List<VehicleImages>> GetVehicleImagesByVehicleIdAsync(int vehicleId)
    {
        return await _repository.GetVehicleImagesByVehicleIdAsync(vehicleId);
    }

    public async Task<VehicleResponse> GetVehicleById(int vehicleId)
    {
        var vehicle = await _repository.GetVehicleById(vehicleId, true, true, true);
        return _mapper.Map<VehicleResponse>(vehicle);
    }
    
    #region private methods

    private async Task AddVehicleImages(IReadOnlyList<string> imageUrls, int vehicleId)
    {
        await _repository.DeleteVehicleImagesByVehicleAsync(vehicleId);
        
        var images = imageUrls.Distinct().ToList()
            .Select<string, VehicleImages>(img => new VehicleImages()
            {
                VehicleId = vehicleId,
                ImageUrl = img
            });
        await _repository.AddVehicleImagesAsync(images);
    }

    #endregion
}