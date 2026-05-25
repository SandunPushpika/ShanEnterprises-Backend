using Application.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using Moq;
using NUnit.Framework;

namespace Test.Application.VehicleServiceTests;

[TestFixture]
public partial class VehicleServiceTests
{
    private Mock<IVehicleRepository> _repositoryMock;
    private Mock<IMapper> _mapperMock;

    private VehicleService _vehicleService;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IVehicleRepository>();
        _mapperMock = new Mock<IMapper>();

        _vehicleService = new VehicleService(
            _repositoryMock.Object,
            _mapperMock.Object
        );
    }
}