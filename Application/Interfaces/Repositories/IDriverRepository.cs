using Core.DTOs.Request.Driver;
using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IDriverRepository
{
    Task<Driver> AddDriverAsync(Driver driver);
    Task UpdateDriverAsync(Driver driver);
    Task<Driver?> GetDriverByIdAsync(int id);
    Task<Driver?> GetDriverByUserIdAsync(long userId);
    Task<bool> HasActiveRequestAsync(long userId);
    Task<(IReadOnlyCollection<Driver> Drivers, int Total)> GetDriversByStatusAsync(DriverSearchRequest request);
    Task<IReadOnlyCollection<Driver>> GetAvailableDriversAsync();
}
