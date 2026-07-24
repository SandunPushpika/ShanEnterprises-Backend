using Application.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Core.DTOs.Request;
using System.Linq;
using Core.Enums;

namespace Application.Repositories;

public class VehicleRepository(AppDbContext context) : IVehicleRepository
{
    public async Task<int> AddVehicle(Vehicle vehicle)
    {
        var res = context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        return res.Entity.Id;
    }
    public async Task<Vehicle?> GetVehicleById(int id, bool includeTypes = false, bool includeImages = false, bool includeBrands = false)
    {
        IQueryable<Vehicle> query = context.Vehicles;

        if (includeTypes)
        {
            query = query.Include(v => v.Type);
        }

        if (includeBrands)
        {
            query = query.Include(v => v.Brand);
        }

        if (includeImages)
        {
            query = query.Include(v => v.VehicleImages);
        }

        return await query.FirstOrDefaultAsync(v => v.Id == id);
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
        if (request.MaxPrice.HasValue && request.MaxPrice != 0)
            query = query.Where(v => v.DailyRentalPrice <= request.MaxPrice.Value);
        if (request.TypeId.HasValue && request.TypeId != 0)
            query = query.Where(v => v.TypeId == request.TypeId.Value);
        if (request.Status.HasValue)
            query = query.Where(v => v.Status == request.Status.Value);
        if (request.MinPassengers is > 0)
            query = query.Where(v => v.SeatCapacity >= request.MinPassengers.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(v =>
                (v.Brand != null && EF.Functions.ILike(v.Brand.Name, $"%{request.Search}%")) ||
                EF.Functions.ILike(v.Model, $"%{request.Search}%"));
        }
        if(request.VehicleIds != null && request.VehicleIds.Any())
            query = query.Where(v => request.VehicleIds.Contains(v.Id));
        
        query = query.Where(v => v.Status != VehicleStatus.UNAVAILABLE);
        
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

    public async Task<IReadOnlyCollection<VehicleBrand>> GetAllVehicleBrands()
    {
        var result = await context.VehicleBrands.ToListAsync();
        return result;
    }

    public async Task<IReadOnlyCollection<VehicleType>> GetAllVehicleTypes()
    {
        var result = await context.VehicleTypes.ToListAsync();
        return result;
    }

    public async Task AddVehicleImagesAsync(IEnumerable<VehicleImages> vehicleImages)
    {
        await context.VehicleImages.AddRangeAsync(vehicleImages);
        await context.SaveChangesAsync();
    }

    public async Task DeleteVehicleImagesByVehicleAsync(int vehicleId)
    {
        await context.VehicleImages
            .Where(v => v.VehicleId == vehicleId)
            .ExecuteDeleteAsync();
        await context.SaveChangesAsync();
    }

    public async Task<List<VehicleImages>> GetVehicleImagesByVehicleIdAsync(int vehicleId)
    {
        var result = await context.VehicleImages.Where(v => v.VehicleId == vehicleId)
            .ToListAsync();
        return result;
    }
}