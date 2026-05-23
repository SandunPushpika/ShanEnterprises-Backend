using Application.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Database;

namespace Application.Repositories;

public class VehicleRepository(AppDbContext context):IVehicleRepository
{
    public async Task AddVehicle(Vehicle vehicle)
    {
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();
    }
}