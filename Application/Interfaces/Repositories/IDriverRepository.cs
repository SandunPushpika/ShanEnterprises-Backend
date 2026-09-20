using Core.DTOs.Request.Driver;
using Core.DTOs.Response;
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
    Task<IReadOnlyCollection<Driver>> GetAvailableDriversAsync(DateTime pickupDatetime, DateTime returnDatetime);
    Task<IReadOnlyCollection<Booking>> GetDriverTripsAsync(int driverId);
    Task<(IReadOnlyCollection<Booking> Trips, int Total)> GetDriverTripsPaginatedAsync(int driverId, int pageNumber, int pageSize);
    Task AddDriverBookingCancellationAsync(DriverBookingCancellation cancellation);
    Task<DriverStatsResponse> GetDriverStatsAsync();
}

