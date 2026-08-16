using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using AutoMapper;
using Infrastructure.Interfaces;
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
    private Mock<IPaymentService> _paymentServiceMock;
    private Mock<IPaymentRepository> _paymentRepositoryMock;

    private BookingService _bookingService;

    private Mock<IDriverRepository> _driverRepositoryMock;
    private Mock<IEmailService> _emailServiceMock;
    private Mock<Microsoft.Extensions.Options.IOptions<Core.Helpers.AppSettings>> _optionsMock;
    private Mock<IUserRepository> _userRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _mapperMock = new Mock<IMapper>();
        _applicationContextMock = new Mock<IContextService>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _optionsMock = new Mock<Microsoft.Extensions.Options.IOptions<Core.Helpers.AppSettings>>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _optionsMock.Setup(x => x.Value).Returns(new Core.Helpers.AppSettings());

        _bookingService = new BookingService(
            _bookingRepositoryMock.Object,
            _vehicleRepositoryMock.Object,
            _driverRepositoryMock.Object,
            _mapperMock.Object,
            _applicationContextMock.Object,
            _paymentServiceMock.Object,
            _paymentRepositoryMock.Object,
            _emailServiceMock.Object,
            _optionsMock.Object,
            _userRepositoryMock.Object
        );
    }
}
