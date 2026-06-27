using Core.DTOs.Request;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Test.Application.VehicleServiceTests;

public partial class VehicleServiceTests
{
    [Test]
    public async Task SearchVehicles_ShouldReturnPagedSearchResponse()
    {
        // Arrange
        var request = new VehicleSearchRequest
        {
            PageNumber = 2,
            PageSize = 10
        };

        var vehiclesList = new List<Vehicle> { new Vehicle { Id = 1 }, new Vehicle { Id = 2 } };
        var vehicleResponsesList = new List<VehicleResponse> { new VehicleResponse { Id = 1 }, new VehicleResponse { Id = 2 } };

        _repositoryMock
            .Setup(x => x.SearchVehicles(request))
            .ReturnsAsync((vehiclesList, 20));

        _mapperMock
            .Setup(x => x.Map<IReadOnlyCollection<VehicleResponse>>(vehiclesList))
            .Returns(vehicleResponsesList);

        // Act
        var result = await _vehicleService.SearchVehicles(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEquivalentTo(vehicleResponsesList);
        result.Total.Should().Be(20);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
    }

    [Test]
    public async Task GetAllBrands_ShouldReturnBrandsFromRepository()
    {
        // Arrange
        var brands = new List<VehicleBrand> { new VehicleBrand { Id = 1, Name = "Toyota" } };
        _repositoryMock
            .Setup(x => x.GetAllVehicleBrands())
            .ReturnsAsync(brands);

        // Act
        var result = await _vehicleService.GetAllBrands();

        // Assert
        result.Should().BeEquivalentTo(brands);
    }

    [Test]
    public async Task GetAllVehicleTypes_ShouldReturnTypesFromRepository()
    {
        // Arrange
        var types = new List<VehicleType> { new VehicleType { Id = 1, Name = "Sedan" } };
        _repositoryMock
            .Setup(x => x.GetAllVehicleTypes())
            .ReturnsAsync(types);

        // Act
        var result = await _vehicleService.GetAllVehicleTypes();

        // Assert
        result.Should().BeEquivalentTo(types);
    }

    [Test]
    public async Task GetVehicleImagesByVehicleIdAsync_ShouldReturnImagesFromRepository()
    {
        // Arrange
        var images = new List<VehicleImages> { new VehicleImages { Id = 1, VehicleId = 5, ImageUrl = "url" } };
        _repositoryMock
            .Setup(x => x.GetVehicleImagesByVehicleIdAsync(5))
            .ReturnsAsync(images);

        // Act
        var result = await _vehicleService.GetVehicleImagesByVehicleIdAsync(5);

        // Assert
        result.Should().BeEquivalentTo(images);
    }

    [Test]
    public async Task GetVehicleById_ShouldReturnMappedResponseFromRepository()
    {
        // Arrange
        var vehicle = new Vehicle { Id = 1, Model = "Civic" };
        var vehicleResponse = new VehicleResponse { Id = 1, Model = "Civic" };

        _repositoryMock
            .Setup(x => x.GetVehicleById(1, true, true, true))
            .ReturnsAsync(vehicle);

        _mapperMock
            .Setup(x => x.Map<VehicleResponse>(vehicle))
            .Returns(vehicleResponse);

        // Act
        var result = await _vehicleService.GetVehicleById(1);

        // Assert
        result.Should().BeEquivalentTo(vehicleResponse);
    }
}
