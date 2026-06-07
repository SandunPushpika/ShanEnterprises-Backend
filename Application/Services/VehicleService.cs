using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request;
using Core.Entities;
using Core.Exceptions;
using Core.DTOs.Response;

namespace Application.Services;

public class VehicleService:IVehicleService
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
        
        await _repository.AddVehicle(vehicle);
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
    }}