using Core.DTOs.Request;
using Core.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Test.Application.VehicleServiceTests;

public partial class VehicleServiceTests
{
    [Test]
    public async Task AddVehicle_ShouldMapRequestAndSaveVehicle()
    {
        // Arrange
        var request = new VehicleCreateRequest
        {
            Model = "Civic",
            RegistrationNumber = "ABC-123",
            ManufactureYear = 2020
        };

        var mappedVehicle = new Vehicle
        {
            Model = request.Model,
            RegistrationNumber = request.RegistrationNumber,
            ManufactureYear = request.ManufactureYear
        };

        _mapperMock
            .Setup(x => x.Map<Vehicle>(request))
            .Returns(mappedVehicle);

        _repositoryMock
            .Setup(x => x.AddVehicle(It.IsAny<Vehicle>()))
            .Returns(Task.FromResult(1));

        // Act
        await _vehicleService.AddVehicle(request);

        // Assert
        _repositoryMock.Verify(x => x.AddVehicle(It.Is<Vehicle>(v =>
            v.Model == request.Model &&
            v.RegistrationNumber == request.RegistrationNumber &&
            v.ManufactureYear == request.ManufactureYear
        )), Times.Once);
    }

    [Test]
    public async Task AddVehicle_ShouldSetCreatedAndUpdatedDates()
    {
        // Arrange
        var request = new VehicleCreateRequest
        {
            Model = "BMW",
            RegistrationNumber = "XYZ-999",
            ManufactureYear = 2022
        };

        var mappedVehicle = new Vehicle
        {
            Model = request.Model
        };

        _mapperMock
            .Setup(x => x.Map<Vehicle>(request))
            .Returns(mappedVehicle);

        Vehicle? capturedVehicle = null;

        _repositoryMock
            .Setup(x => x.AddVehicle(It.IsAny<Vehicle>()))
            .Callback<Vehicle>(v => capturedVehicle = v)
            .Returns(Task.FromResult(1));

        // Act
        await _vehicleService.AddVehicle(request);

        // Assert
        capturedVehicle.Should().NotBeNull();
        capturedVehicle!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        capturedVehicle.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Test]
    public async Task AddVehicle_ShouldAddVehicleImages_WhenImageUrlsAreProvided()
    {
        // Arrange
        var request = new VehicleCreateRequest
        {
            Model = "Civic",
            RegistrationNumber = "ABC-123",
            ManufactureYear = 2020,
            ImageUrls = new List<string> { "url1", "url2", "url1" } // contains duplicate to test Distinct
        };

        var mappedVehicle = new Vehicle
        {
            Model = request.Model,
            RegistrationNumber = request.RegistrationNumber
        };

        _mapperMock
            .Setup(x => x.Map<Vehicle>(request))
            .Returns(mappedVehicle);

        _repositoryMock
            .Setup(x => x.AddVehicle(It.IsAny<Vehicle>()))
            .ReturnsAsync(5); // Return vehicle ID 5

        // Act
        await _vehicleService.AddVehicle(request);

        // Assert
        _repositoryMock.Verify(x => x.DeleteVehicleImagesByVehicleAsync(5), Times.Once);
        _repositoryMock.Verify(x => x.AddVehicleImagesAsync(It.Is<IEnumerable<VehicleImages>>(imgs =>
            imgs.Count() == 2 &&
            imgs.All(i => i.VehicleId == 5) &&
            imgs.Any(i => i.ImageUrl == "url1") &&
            imgs.Any(i => i.ImageUrl == "url2")
        )), Times.Once);
    }
}
