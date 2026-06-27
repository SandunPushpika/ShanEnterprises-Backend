using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Test.Application.VehicleServiceTests;

public partial class VehicleServiceTests
{
    [Test]
    public async Task DeleteVehicle_ShouldThrowException_WhenVehicleNotFound()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetVehicleById(1, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Vehicle?)null);

        // Act
        Func<Task> act = async () => await _vehicleService.DeleteVehicle(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Vehicle with id 1 not found");
    }

    [Test]
    public async Task DeleteVehicle_ShouldThrowException_WhenVehicleAlreadyUnavailable()
    {
        // Arrange
        var existingVehicle = new Vehicle
        {
            Id = 1,
            Status = VehicleStatus.UNAVAILABLE
        };

        _repositoryMock
            .Setup(x => x.GetVehicleById(1, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(existingVehicle);

        // Act
        Func<Task> act = async () => await _vehicleService.DeleteVehicle(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Vehicle with id 1 not found");
    }

    [Test]
    public async Task DeleteVehicle_ShouldMarkVehicleAsUnavailableAndUpdate_WhenVehicleExistsAndIsAvailable()
    {
        // Arrange
        var existingVehicle = new Vehicle
        {
            Id = 1,
            Status = VehicleStatus.AVAILABLE,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _repositoryMock
            .Setup(x => x.GetVehicleById(1, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(existingVehicle);

        // Act
        await _vehicleService.DeleteVehicle(1);

        // Assert
        existingVehicle.Status.Should().Be(VehicleStatus.UNAVAILABLE);
        existingVehicle.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        _repositoryMock.Verify(x => x.UpdateVehicle(existingVehicle), Times.Once);
    }
}
