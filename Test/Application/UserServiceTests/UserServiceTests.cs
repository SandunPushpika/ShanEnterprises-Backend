using Core.DTOs.Request.Auth;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services;

public partial class UserServiceTests
{
    #region GetUserByIdAsync

    [Test]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserIsNotOwnerAndNotAdmin()
    {
        // Arrange
        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(new User { Id = 2, Role = UserRole.CUSTOMER });

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserIsOwner()
    {
        // Arrange
        var user = new User { Id = 1, Role = UserRole.ADMIN };

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        _mapperMock.Setup(x => x.Map<UserResponse>(user))
            .Returns(new UserResponse());

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserIsAdmin()
    {
        // Arrange
        var admin = new User { Id = 99, Role = UserRole.ADMIN };
        var targetUser = new User { Id = 1, Role = UserRole.DRIVER };

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(admin);

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(targetUser);

        _mapperMock.Setup(x => x.Map<UserResponse>(targetUser))
            .Returns(new UserResponse());

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(new User { Id = 1, Role = UserRole.ADMIN });

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region UpdateUserAsync

    [Test]
    public async Task UpdateUserAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var request = new UserUpdateRequest { Id = 1 };

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _userService.UpdateUserAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("User not found");
    }

    [Test]
    public async Task UpdateUserAsync_ShouldUpdateAndReturnUser()
    {
        // Arrange
        var request = new UserUpdateRequest
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com"
        };

        var existingUser = new User
        {
            Id = 1,
            FirstName = "Old",
            LastName = "Name",
            Email = "old@test.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updatedUser = new User
        {
            Id = 1,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            CreatedAt = existingUser.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(updatedUser);

        _mapperMock.Setup(x => x.Map<UserResponse>(updatedUser))
            .Returns(new UserResponse());

        // Act
        var result = await _userService.UpdateUserAsync(request);

        // Assert
        result.Should().NotBeNull();

        _userRepositoryMock.Verify(x => x.UpdateUserAsync(It.Is<User>(u =>
            u.FirstName == request.FirstName &&
            u.LastName == request.LastName &&
            u.Email == request.Email
        )), Times.Once);
    }

    #endregion
}