using Core.DTOs.Request;

namespace Application.Interfaces.Services;

public interface IVehicleService
{
    Task AddVehicle(VehicleCreateRequest request);
}