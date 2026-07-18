using Application.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Core.DTOs.Request;
using System.Linq;
using Core.DTOs.Request.Maintenance;
using Core.DTOs.Response;
using Core.Enums;

namespace Application.Repositories;

public class VehicleMaintenanceRepository(AppDbContext context) : IVehicleMaintenanceRepository
{
    public async Task AddVehicleMaintenance(VehicleMaintenance vehicleMaintenance)
    {
        context.VehicleMaintenances.Add(vehicleMaintenance);
        await context.SaveChangesAsync();
    }
    
    public async Task<(IReadOnlyCollection<VehicleMaintenance> VehicleMaintenances, int Total)> GetAllVehicleMaintenances()
    {
        var query = context.VehicleMaintenances
        .Include(m => m.Vehicle)
        .ThenInclude(v => v.Brand)
        .AsQueryable();
        var total = await query.CountAsync();
        
        var maintenances = await query
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        
        return (maintenances, total);
    }

    public async Task UpdateVehicleMaintenance(VehicleMaintenance vehicleMaintenance)
    {
        context.VehicleMaintenances.Update(vehicleMaintenance);
        
        await context.SaveChangesAsync();
    }

    public async Task<VehicleMaintenance?> GetVehicleMaintenanceById(int id)
    {
    
         return await context.VehicleMaintenances
            .Include(m => m.Vehicle)
            .ThenInclude(v => v.Brand)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task DeleteVehicleMaintenance(VehicleMaintenance vehicleMaintenance)
    {
        context.VehicleMaintenances.Remove(vehicleMaintenance);
        await context.SaveChangesAsync();
    }

    public async Task<(IReadOnlyCollection<VehicleMaintenance> VehicleMaintenances, int Total)> SearchVehicleMaintenances(MaintenanceSearchRequest request)
    {
        var query = context.VehicleMaintenances
            .Include(m => m.Vehicle)
            .ThenInclude(v => v.Brand)
            .AsQueryable();

        if (request.VehicleId.HasValue && request.VehicleId > 0)
            query = query.Where(m => m.VehicleId == request.VehicleId.Value);

        if (request.Year.HasValue)
            query = query.Where(m => m.MaintenanceStart.Year == request.Year.Value);

        if (request.Month.HasValue)
            query = query.Where(m => m.MaintenanceStart.Month == request.Month.Value);

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<MaintenanceStatus>(request.Status, out var statusEnum))
            query = query.Where(m => m.Status == statusEnum);

        var total = await query.CountAsync();
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        var results = await query
            .OrderByDescending(m => m.MaintenanceStart)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    return (results, total);
    }

    public async Task<IReadOnlyCollection<MaintenanceStatsResponse>> GetMaintenanceStats(string groupBy, int? year)
    {

        var query = context.VehicleMaintenances.AsQueryable();
    
        if (year.HasValue)
        {
            query = query.Where(m => m.MaintenanceStart.Year == year.Value);
        }
    
        var all = await query.ToListAsync();

        IEnumerable<MaintenanceStatsResponse> stats = groupBy.ToLower() switch
        {
            "year" => all
                .GroupBy(m => m.MaintenanceStart.Year)
                .OrderBy(g => g.Key)
                .Select(g => new MaintenanceStatsResponse
                {
                    Label = g.Key.ToString(),
                    TotalCost = g.Sum(m => m.Cost),
                    RecordCount = g.Count()
                }),

            "week" => all
                .GroupBy(m => System.Globalization.ISOWeek.GetWeekOfYear(m.MaintenanceStart.ToDateTime(TimeOnly.MinValue)))
                .OrderBy(g => g.Key)
                .Select(g => new MaintenanceStatsResponse
                {
                    Label = $"Week {g.Key}",
                    TotalCost = g.Sum(m => m.Cost),
                    RecordCount = g.Count()
                }),

            _ => all  
                .GroupBy(m => new { m.MaintenanceStart.Year, m.MaintenanceStart.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MaintenanceStatsResponse
                {
                    Label = $"{System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)} {g.Key.Year}",
                    TotalCost = g.Sum(m => m.Cost),
                    RecordCount = g.Count()
                })
        };

    return stats.ToList();
    }
}