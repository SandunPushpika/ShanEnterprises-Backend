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
    public async Task<Vehicle?> GetVehicleById(int id)
    {
        return await context.Vehicles.FindAsync(id);
    }

    public async Task UpdateVehicle(Vehicle vehicle)
    {
        context.Vehicles.Update(vehicle);
        await context.SaveChangesAsync();
    }
}