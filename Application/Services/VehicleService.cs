using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request;
using Core.Entities;

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
}