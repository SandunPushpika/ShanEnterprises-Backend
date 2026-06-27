using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;
using Core.Entities;
using Core.Exceptions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Test.Application.BookingServiceTests;

public partial class BookingServiceTests
{
    #region AddBooking

    [Test]
    public async Task AddBooking_ShouldThrowException_WhenUserNotLoggedIn()
    {
        // Arrange
        _applicationContextMock
            .Setup(x => x.GetUser())
            .Returns(Task.FromResult((User?)null));

        var request = new BookingCreateRequest { VehicleId = 1 };

        // Act
        Func<Task> act = async () => await _bookingService.AddBooking(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User is not logged in to place the booking");
    }

    [Test]
    public async Task AddBooking_ShouldThrowException_WhenVehicleIsAlreadyBooked()
    {
        // Arrange
        var user = new User { Id = 10 };
        _applicationContextMock
            .Setup(x => x.GetUser())
            .Returns(Task.FromResult(user));

        var request = new BookingCreateRequest
        {
            VehicleId = 1,
            PickupDateTime = DateTime.UtcNow.AddDays(1),
            ReturnDateTime = DateTime.UtcNow.AddDays(3)
        };

        _bookingRepositoryMock
            .Setup(x => x.IsBooked(request.VehicleId, request.PickupDateTime, request.ReturnDateTime))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _bookingService.AddBooking(request);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Vehicle is already booked on given date!");
    }

    [Test]
    public async Task AddBooking_ShouldCalculateFieldsAndSaveBooking_WhenRequestIsValid()
    {
        // Arrange
        var user = new User { Id = 10 };
        _applicationContextMock
            .Setup(x => x.GetUser())
            .Returns(Task.FromResult(user));

        var request = new BookingCreateRequest
        {
            VehicleId = 1,
            PickupDateTime = DateTime.UtcNow.AddDays(1),
            ReturnDateTime = DateTime.UtcNow.AddDays(5)
        };

        var mappedBooking = new Booking
        {
            VehicleId = request.VehicleId,
            PickupDatetime = request.PickupDateTime,
            ReturnDatetime = request.ReturnDateTime,
            BaseRentalCost = 100,
            DriverFee = 20,
            TaxAmount = 10,
            DiscountAmount = 5
        };

        _bookingRepositoryMock
            .Setup(x => x.IsBooked(request.VehicleId, request.PickupDateTime, request.ReturnDateTime))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(x => x.Map<Booking>(request))
            .Returns(mappedBooking);

        // Act
        await _bookingService.AddBooking(request);

        // Assert
        mappedBooking.CustomerId.Should().Be(10);
        mappedBooking.RentalDays.Should().Be(4); // 5 - 1 = 4 days
        mappedBooking.TotalAmount.Should().Be(125); // 100 + 20 + 10 - 5 = 125
        mappedBooking.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        mappedBooking.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        mappedBooking.BookingReference.Should().NotBeNullOrEmpty();

        _bookingRepositoryMock.Verify(x => x.AddBooking(mappedBooking), Times.Once);
    }

    #endregion

    #region GetAllBookings

    [Test]
    public async Task GetAllBookings_ShouldReturnPagedSearchResponse()
    {
        // Arrange
        var request = new BookingSearchRequest
        {
            PageNumber = 1,
            PageSize = 5
        };

        var bookingsList = new List<Booking> { new Booking { Id = 1 }, new Booking { Id = 2 } };
        var bookingResponsesList = new List<BookingReadResponse> { new BookingReadResponse { Id = 1 }, new BookingReadResponse { Id = 2 } };

        _bookingRepositoryMock
            .Setup(x => x.GetAllBookings(request))
            .ReturnsAsync((bookingsList, 10));

        _mapperMock
            .Setup(x => x.Map<IReadOnlyCollection<BookingReadResponse>>(bookingsList))
            .Returns(bookingResponsesList);

        // Act
        var result = await _bookingService.GetAllBookings(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEquivalentTo(bookingResponsesList);
        result.Total.Should().Be(10);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(5);
    }

    #endregion

    #region GetBookedDatesByVehicleId

    [Test]
    public async Task GetBookedDatesByVehicleId_ShouldThrowException_WhenVehicleNotFound()
    {
        // Arrange
        _vehicleRepositoryMock
            .Setup(x => x.GetVehicleById(1, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Vehicle?)null);

        // Act
        Func<Task> act = async () => await _bookingService.GetBookedDatesByVehicleId(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Vehicle with id 1 not found");
    }

    [Test]
    public async Task GetBookedDatesByVehicleId_ShouldReturnBookedDates_WhenVehicleExists()
    {
        // Arrange
        var vehicle = new Vehicle { Id = 1 };
        var dateRanges = new List<BookedDateRangeResponse>
        {
            new BookedDateRangeResponse { StartDate = DateTime.UtcNow.ToString(), EndDate = DateTime.UtcNow.AddDays(2).ToString() }
        };

        _vehicleRepositoryMock
            .Setup(x => x.GetVehicleById(1, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(vehicle);

        _bookingRepositoryMock
            .Setup(x => x.GetBookedDatesByVehicleId(1))
            .ReturnsAsync(dateRanges);

        // Act
        var result = await _bookingService.GetBookedDatesByVehicleId(1);

        // Assert
        result.Should().BeEquivalentTo(dateRanges);
    }

    #endregion

    #region Unimplemented Methods

    [Test]
    public void UpdateBooking_ShouldThrowNotImplementedException()
    {
        // Act
        Func<Task> act = async () => await _bookingService.UpdateBooking(1, new BookingUpdateRequest());

        // Assert
        act.Should().ThrowAsync<NotImplementedException>();
    }

    [Test]
    public void GetBookingById_ShouldThrowNotImplementedException()
    {
        // Act
        Func<Task> act = async () => await _bookingService.GetBookingById(1);

        // Assert
        act.Should().ThrowAsync<NotImplementedException>();
    }

    [Test]
    public void UpdateBookingStatus_ShouldThrowNotImplementedException()
    {
        // Act
        Func<Task> act = async () => await _bookingService.UpdateBookingStatus(1, new BookingStatusUpdatRequest());

        // Assert
        act.Should().ThrowAsync<NotImplementedException>();
    }

    [Test]
    public void DeleteBooking_ShouldThrowNotImplementedException()
    {
        // Act
        Func<Task> act = async () => await _bookingService.DeleteBooking(1);

        // Assert
        act.Should().ThrowAsync<NotImplementedException>();
    }

    #endregion
}
