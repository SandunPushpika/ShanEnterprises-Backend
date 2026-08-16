using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Infrastructure.Interfaces;

namespace Test.Application.VehicleServiceTests;

[TestFixture]
public partial class VehicleServiceTests
{
    private Mock<IVehicleRepository> _repositoryMock;
    private Mock<IMapper> _mapperMock;

    private VehicleService _vehicleService;

    private Mock<IRecommendationService> _recommendationServiceMock;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IVehicleRepository>();
        _mapperMock = new Mock<IMapper>();
        _recommendationServiceMock = new Mock<IRecommendationService>();

        _vehicleService = new VehicleService(
            _repositoryMock.Object,
            _recommendationServiceMock.Object,
            _mapperMock.Object
        );
    }
}