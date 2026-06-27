using Core.DTOs.Request;
using Core.Entities;
using Core.Exceptions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Test.Application.VehicleServiceTests;

public partial class VehicleServiceTests
{
    [Test]
    public async Task UpdateVehicle_ShouldThrowException_WhenVehicleNotFound()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetVehicleById(1, false, false, false)) // matching original default arguments if any
            .ReturnsAsync((Vehicle?)null);

        var request = new VehicleUpdateRequest { Model = "New civic" };

        // Act
        Func<Task> act = async () => await _vehicleService.UpdateVehicle(1, request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Vehicle with id 1 not found");
    }

    [Test]
    public async Task UpdateVehicle_ShouldMapRequestAndUpdateVehicleWithoutImages_WhenImageUrlsAreNull()
    {
        // Arrange
        var existingVehicle = new Vehicle
        {
            Id = 1,
            Model = "Old Civic",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var request = new VehicleUpdateRequest { Model = "New Civic", ImageUrls = null! };

        _repositoryMock
            .Setup(x => x.GetVehicleById(1, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(existingVehicle);

        // Act
        await _vehicleService.UpdateVehicle(1, request);

        // Assert
        _mapperMock.Verify(x => x.Map(request, existingVehicle), Times.Once);
        existingVehicle.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        _repositoryMock.Verify(x => x.UpdateVehicle(existingVehicle), Times.Once);
        _repositoryMock.Verify(x => x.DeleteVehicleImagesByVehicleAsync(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task UpdateVehicle_ShouldUpdateVehicleAndImages_WhenImageUrlsAreProvided()
    {
        // Arrange
        var existingVehicle = new Vehicle
        {
            Id = 1,
            Model = "Old Civic",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var request = new VehicleUpdateRequest
        {
            Model = "New Civic",
            ImageUrls = new List<string> { "img1", "img2" }
        };

        _repositoryMock
            .Setup(x => x.GetVehicleById(1, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(existingVehicle);

        // Act
        await _vehicleService.UpdateVehicle(1, request);

        // Assert
        _mapperMock.Verify(x => x.Map(request, existingVehicle), Times.Once);
        _repositoryMock.Verify(x => x.UpdateVehicle(existingVehicle), Times.Once);
        _repositoryMock.Verify(x => x.DeleteVehicleImagesByVehicleAsync(1), Times.Once);
        _repositoryMock.Verify(x => x.AddVehicleImagesAsync(It.Is<IEnumerable<VehicleImages>>(imgs =>
            imgs.Count() == 2 &&
            imgs.All(i => i.VehicleId == 1)
        )), Times.Once);
    }
}
