using Application.Interfaces.Repositories;
using Core.DTOs.Request.Driver;
using Core.Entities;
using Core.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories;

public class DriverRepository(AppDbContext context) : IDriverRepository
{
    public async Task<Driver> AddDriverAsync(Driver driver)
    {
        var result = await context.Drivers.AddAsync(driver);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task UpdateDriverAsync(Driver driver)
    {
        context.Drivers.Update(driver);
        await context.SaveChangesAsync();
    }

    public async Task<Driver?> GetDriverByIdAsync(int id)
    {
        return await context.Drivers
            .Include(d => d.User)
            .Include(d => d.ApprovedByUser)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Driver?> GetDriverByUserIdAsync(long userId)
    {
        return await context.Drivers
            .Include(d => d.User)
            .Include(d => d.ApprovedByUser)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<bool> HasActiveRequestAsync(long userId)
    {
        return await context.Drivers.AnyAsync(d =>
            d.UserId == userId &&
            d.DriverStatus != DriverStatus.REJECTED &&
            d.DriverStatus != DriverStatus.DEACTIVATED);
    }

    public async Task<(IReadOnlyCollection<Driver> Drivers, int Total)> GetDriversByStatusAsync(DriverSearchRequest request)
    {
        var query = context.Drivers
            .Include(d => d.User)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(d => d.DriverStatus == request.Status.Value);

        var total = await query.CountAsync();
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize   = request.PageSize   < 1 ? 10 : request.PageSize;

        var drivers = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (drivers, total);
    }
}
