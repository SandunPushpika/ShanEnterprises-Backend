using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IVehicleRepository
{
    Task AddVehicle(Vehicle vehicle);
}