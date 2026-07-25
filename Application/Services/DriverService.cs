using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Driver;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;

namespace Application.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _repository;
    private readonly IContextService _context;
    private readonly IMapper _mapper;

    public DriverService(
        IDriverRepository repository,
        IContextService context,
        IMapper mapper)
    {
        _repository = repository;
        _context    = context;
        _mapper     = mapper;
    }

    public async Task SubmitDriverRequestAsync(DriverCreateRequest createRequest)
    {
        var user = await _context.GetUser();

        var hasActive = await _repository.HasActiveRequestAsync(user.Id);
        if (hasActive)
            throw new FailedOperationException(
                "You already have an active or pending driver request.");

        var driver = _mapper.Map<Driver>(createRequest);
        driver.UserId     = user.Id;
        driver.DriverStatus = DriverStatus.PENDING;
        driver.Availability = AvailabilityStatus.AVAILABLE;
        driver.CreatedAt  = DateTime.UtcNow;
        driver.UpdatedAt  = DateTime.UtcNow;

        await _repository.AddDriverAsync(driver);
    }

    public async Task ApproveDriverAsync(int driverId)
    {
        var driver = await _repository.GetDriverByIdAsync(driverId);
        if (driver == null)
            throw new NotFoundException($"Driver with id {driverId} not found.");

        if (driver.DriverStatus != DriverStatus.PENDING)
            throw new FailedOperationException(
                $"Only PENDING requests can be approved. Current status: {driver.DriverStatus}.");

        var admin = await _context.GetUser();

        driver.DriverStatus = DriverStatus.APPROVED;
        driver.ApprovedBy   = admin.Id;
        driver.ApprovedAt   = DateTime.UtcNow;
        driver.UpdatedAt    = DateTime.UtcNow;
        driver.CreatedAt    = DateTime.SpecifyKind(driver.CreatedAt, DateTimeKind.Utc);

        await _repository.UpdateDriverAsync(driver);
    }

    public async Task RejectDriverAsync(int driverId)
    {
        var driver = await _repository.GetDriverByIdAsync(driverId);
        if (driver == null)
            throw new NotFoundException($"Driver with id {driverId} not found.");

        if (driver.DriverStatus == DriverStatus.DEACTIVATED)
            throw new FailedOperationException("Driver is already deactivated.");

        // PENDING → REJECTED  |  APPROVED → DEACTIVATED
        driver.DriverStatus = driver.DriverStatus == DriverStatus.PENDING
            ? DriverStatus.REJECTED
            : DriverStatus.DEACTIVATED;

        driver.UpdatedAt = DateTime.UtcNow;
        driver.CreatedAt = DateTime.SpecifyKind(driver.CreatedAt, DateTimeKind.Utc);

        await _repository.UpdateDriverAsync(driver);
    }

    public async Task<SearchResponse<DriverResponse>> GetDriversByStatusAsync(DriverSearchRequest request)
    {
        var (drivers, total) = await _repository.GetDriversByStatusAsync(request);
        var driverResponses  = _mapper.Map<IReadOnlyCollection<DriverResponse>>(drivers);

        return new SearchResponse<DriverResponse>
        {
            Data       = driverResponses,
            Total      = total,
            PageNumber = request.PageNumber,
            PageSize   = request.PageSize
        };
    }

    public async Task<DriverStatusResponse> GetMyDriverStatusAsync()
    {
        var user   = await _context.GetUser();
        var driver = await _repository.GetDriverByUserIdAsync(user.Id);

        if (driver == null)
            return new DriverStatusResponse
            {
                HasRequest = false,
                Message    = "No driver request found for your account."
            };

        return new DriverStatusResponse
        {
            DriverId   = driver.Id,
            Status     = driver.DriverStatus,
            HasRequest = true,
            Message    = $"Your driver request status is {driver.DriverStatus}."
        };
    }

    public async Task<IReadOnlyCollection<DriverResponse>> GetAvailableDriversAsync()
    {
        var drivers = await _repository.GetAvailableDriversAsync();
        return _mapper.Map<IReadOnlyCollection<DriverResponse>>(drivers);
    }

    public async Task<DriverResponse> GetDriverByIdAsync(int driverId)
    {
        var driver = await _repository.GetDriverByIdAsync(driverId);
        if (driver == null)
            throw new NotFoundException($"Driver with id {driverId} not found.");
            
        return _mapper.Map<DriverResponse>(driver);
    }
}
