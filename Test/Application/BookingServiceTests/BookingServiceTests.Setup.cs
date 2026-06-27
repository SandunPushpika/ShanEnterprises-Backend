using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using AutoMapper;
using Moq;
using NUnit.Framework;

namespace Test.Application.BookingServiceTests;

[TestFixture]
public partial class BookingServiceTests
{
    private Mock<IBookingRepository> _bookingRepositoryMock;
    private Mock<IVehicleRepository> _vehicleRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IContextService> _applicationContextMock;

    private BookingService _bookingService;

    [SetUp]
    public void Setup()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _mapperMock = new Mock<IMapper>();
        _applicationContextMock = new Mock<IContextService>();

        _bookingService = new BookingService(
            _bookingRepositoryMock.Object,
            _vehicleRepositoryMock.Object,
            _mapperMock.Object,
            _applicationContextMock.Object
        );
    }
}
