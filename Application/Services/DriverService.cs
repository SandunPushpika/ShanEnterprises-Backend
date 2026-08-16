using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Driver;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;

using Core.Helpers;
using Core.Helpers.Templates;
using Microsoft.Extensions.Options;
using Infrastructure.Interfaces;

namespace Application.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _repository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IContextService _context;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly AppSettings _appSettings;

    public DriverService(
        IDriverRepository repository,
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IContextService context,
        IMapper mapper,
        IEmailService emailService,
        IOptions<AppSettings> appSettings)
    {
        _repository        = repository;
        _bookingRepository = bookingRepository;
        _userRepository    = userRepository;
        _context           = context;
        _mapper            = mapper;
        _emailService      = emailService;
        _appSettings       = appSettings.Value;
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

        if (driver.User != null)
        {
            driver.User.Role = UserRole.DRIVER;
            driver.User.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var user = await _userRepository.GetUserByIdAsync((int)driver.UserId);
            if (user != null)
            {
                user.Role = UserRole.DRIVER;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user);
            }
        }

        await _repository.UpdateDriverAsync(driver);
    }

    public async Task RejectDriverAsync(int driverId)
    {
        var driver = await _repository.GetDriverByIdAsync(driverId);
        if (driver == null)
            throw new NotFoundException($"Driver with id {driverId} not found.");

        if (driver.DriverStatus == DriverStatus.APPROVED)
        {
            // Check for active/pending bookings
            var blockingStatuses = new[] { BookingStatus.PENDING, BookingStatus.CONFIRMED, BookingStatus.ONGOING };
            var hasBlockingBookings = await _bookingRepository.HasActiveBookingsForDriverAsync(driver.Id, blockingStatuses);
            if (hasBlockingBookings)
                throw new FailedOperationException(
                    "This driver has active or pending bookings and cannot be deactivated. Please cancel or reassign those bookings first.");
        }

        if (driver.DriverStatus == DriverStatus.DEACTIVATED)
            throw new FailedOperationException("Driver is already deactivated.");

        // PENDING → REJECTED  |  APPROVED → DEACTIVATED
        driver.DriverStatus = driver.DriverStatus == DriverStatus.PENDING
            ? DriverStatus.REJECTED
            : DriverStatus.DEACTIVATED;

        driver.UpdatedAt = DateTime.UtcNow;
        driver.CreatedAt = DateTime.SpecifyKind(driver.CreatedAt, DateTimeKind.Utc);

        // Revert user role to CUSTOMER if deactivated
        if (driver.DriverStatus == DriverStatus.DEACTIVATED)
        {
            if (driver.User != null)
            {
                driver.User.Role = UserRole.CUSTOMER;
                driver.User.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var user = await _userRepository.GetUserByIdAsync((int)driver.UserId);
                if (user != null)
                {
                    user.Role = UserRole.CUSTOMER;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateUserAsync(user);
                }
            }
        }

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

    public async Task<IReadOnlyCollection<DriverResponse>> GetAvailableDriversAsync(AvailableDriverRequest request)
    {
        var drivers = await _repository.GetAvailableDriversAsync(
            request.PickupDatetime,
            request.ReturnDatetime);
        return _mapper.Map<IReadOnlyCollection<DriverResponse>>(drivers);
    }

    public async Task<DriverResponse> GetDriverByIdAsync(int driverId)
    {
        var driver = await _repository.GetDriverByIdAsync(driverId);
        if (driver == null)
            throw new NotFoundException($"Driver with id {driverId} not found.");
            
        return _mapper.Map<DriverResponse>(driver);
    }

    public async Task<SearchResponse<BookingReadResponse>> GetMyTripsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var user = await _context.GetUser();
        var driver = await _repository.GetDriverByUserIdAsync(user.Id);
        if (driver == null)
            return new SearchResponse<BookingReadResponse> { Data = new List<BookingReadResponse>(), Total = 0, PageNumber = pageNumber, PageSize = pageSize };

        var (trips, total) = await _repository.GetDriverTripsPaginatedAsync(driver.Id, pageNumber, pageSize);
        var responses = _mapper.Map<IReadOnlyCollection<BookingReadResponse>>(trips);
        return new SearchResponse<BookingReadResponse> { Data = responses, Total = total, PageNumber = pageNumber, PageSize = pageSize };
    }

    public async Task<SearchResponse<BookingReadResponse>> GetDriverTripsAsync(int driverId, int pageNumber = 1, int pageSize = 10)
    {
        var driver = await _repository.GetDriverByIdAsync(driverId);
        if (driver == null)
            throw new NotFoundException($"Driver with id {driverId} not found.");
        var (trips, total) = await _repository.GetDriverTripsPaginatedAsync(driverId, pageNumber, pageSize);
        var responses = _mapper.Map<IReadOnlyCollection<BookingReadResponse>>(trips);
        return new SearchResponse<BookingReadResponse> { Data = responses, Total = total, PageNumber = pageNumber, PageSize = pageSize };
    }

    public async Task<DriverTripCancelResponse> CancelTripAssignmentAsync(int bookingId)
    {
        var user = await _context.GetUser();
        var driver = await _repository.GetDriverByUserIdAsync(user.Id);
        if (driver == null)
            throw new NotFoundException("Driver profile not found for current user.");

        var booking = await _bookingRepository.GetBookingById(bookingId);
        if (booking == null)
            throw new NotFoundException($"Booking with id {bookingId} not found.");

        if (booking.DriverId != driver.Id)
            throw new FailedOperationException("You are not assigned as the driver for this trip.");

        if (booking.BookingStatus == BookingStatus.CANCELLED || booking.BookingStatus == BookingStatus.COMPLETED)
            throw new FailedOperationException($"Cannot cancel driver assignment for a {booking.BookingStatus} trip.");

        // Record cancellation
        var cancellation = new DriverBookingCancellation
        {
            BookingId = booking.Id,
            DriverId = driver.Id,
            CancelledAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        await _repository.AddDriverBookingCancellationAsync(cancellation);

        // Find available alternative drivers for the booking's time window
        var availableDrivers = await _repository.GetAvailableDriversAsync(booking.PickupDatetime, booking.ReturnDatetime);
        var candidate = availableDrivers.FirstOrDefault(d => d.Id != driver.Id);

        if (candidate != null)
        {
            booking.DriverId = candidate.Id;
        }
        else
        {
            booking.DriverId = null;
        }

        booking.UpdatedAt = DateTime.UtcNow;
        booking.PickupDatetime = DateTime.SpecifyKind(booking.PickupDatetime, DateTimeKind.Utc);
        booking.ReturnDatetime = DateTime.SpecifyKind(booking.ReturnDatetime, DateTimeKind.Utc);
        booking.CreatedAt = DateTime.SpecifyKind(booking.CreatedAt, DateTimeKind.Utc);
        
        await _bookingRepository.UpdateBooking(booking);

        // Send email
        try
        {
            var customer = await _userRepository.GetUserByIdAsync((int)booking.CustomerId);
            if (customer != null)
            {
                var bookingsUrl = string.IsNullOrWhiteSpace(_appSettings.BookingsUrl) ? "your bookings page" : _appSettings.BookingsUrl;
                var emailHtml = BookingEmailTemplates.GenerateDriverCancellationEmail(
                    customer.FirstName,
                    booking.BookingReference,
                    booking.Vehicle?.Model ?? "your reserved vehicle",
                    booking.PickupDatetime.ToString("MMM dd, yyyy HH:mm"),
                    booking.ReturnDatetime.ToString("MMM dd, yyyy HH:mm"),
                    bookingsUrl);
                
                await _emailService.SendEmailAsync(_appSettings.MailSettings, new Core.DTOs.Request.Other.EmailSendRequest
                {
                    To = customer.Email,
                    Subject = "Important: Driver Update for Your Booking",
                    Body = emailHtml,
                    IsBodyHtml = true
                });
            }
        }
        catch { }

        if (candidate != null)
        {
            return new DriverTripCancelResponse
            {
                Reassigned = true,
                NewDriverId = candidate.Id,
                NewDriverName = candidate.User != null ? $"{candidate.User.FirstName} {candidate.User.LastName}" : $"Driver #{candidate.Id}",
                Message = $"Trip assignment cancelled. Successfully reassigned trip to driver {(candidate.User != null ? candidate.User.FirstName + " " + candidate.User.LastName : "#" + candidate.Id)}."
            };
        }
        else
        {
            return new DriverTripCancelResponse
            {
                Reassigned = false,
                Message = "Trip assignment cancelled. No alternative available driver was found at this time."
            };
        }
    }
}
