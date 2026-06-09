using Application.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Core.DTOs.Request;
using System.Linq;

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
    
    public async Task<(IReadOnlyCollection<Vehicle> Vehicles, int Total)> SearchVehicles(VehicleSearchRequest request)
    {
        var query = context.Vehicles
            .Include(v => v.Brand)
            .Include(v => v.Type)
            .AsQueryable();
        if (request.MinPrice.HasValue)
            query = query.Where(v => v.DailyRentalPrice >= request.MinPrice.Value);
        if (request.MaxPrice.HasValue)
            query = query.Where(v => v.DailyRentalPrice <= request.MaxPrice.Value);
        if (request.TypeId.HasValue)
            query = query.Where(v => v.TypeId == request.TypeId.Value);
        if (request.Status.HasValue)
            query = query.Where(v => v.Status == request.Status.Value);
        if (request.MinPassengers.HasValue)
            query = query.Where(v => v.SeatCapacity >= request.MinPassengers.Value);
        var total = await query.CountAsync();
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        var vehicles = await query
            .OrderBy(v => v.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (vehicles, total);
    }
}