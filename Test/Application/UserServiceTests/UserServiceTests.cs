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

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(new User { Id = 1, Role = UserRole.CUSTOMER });

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

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(new User { Id = 1, Role = UserRole.CUSTOMER });

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

    #region Profile Operations

    [Test]
    public async Task GetCurrentUserProfileAsync_ShouldReturnProfile_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" };

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        _mapperMock.Setup(x => x.Map<UserResponse>(user))
            .Returns(new UserResponse { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" });

        // Act
        var result = await _userService.GetCurrentUserProfileAsync();

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Jane");
        result.Email.Should().Be("jane@example.com");
    }

    [Test]
    public async Task UpdateCurrentUserProfileAsync_ShouldUpdateFieldsAndReturnResponse()
    {
        // Arrange
        var user = new User { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" };
        var updateRequest = new Core.DTOs.Request.User.UserProfileUpdateRequest
        {
            FirstName = "Janet",
            LastName = "Smith",
            PhoneNumber = "0771234567",
            City = "Colombo",
            Address = "123 Main Street",
            NicPassportNumber = "199012345678"
        };

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(x => x.Map<UserResponse>(user))
            .Returns(new UserResponse
            {
                Id = 1,
                FirstName = "Janet",
                LastName = "Smith",
                PhoneNumber = "0771234567",
                City = "Colombo",
                Address = "123 Main Street",
                NicPassportNumber = "199012345678"
            });

        // Act
        var result = await _userService.UpdateCurrentUserProfileAsync(updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Janet");
        _userRepositoryMock.Verify(x => x.UpdateUserAsync(It.Is<User>(u =>
            u.FirstName == "Janet" &&
            u.LastName == "Smith" &&
            u.PhoneNumber == "0771234567"
        )), Times.Once);
    }

    [Test]
    public async Task UpdateProfileImageAsync_ShouldUploadAndPersistUrl()
    {
        // Arrange
        var user = new User { Id = 1, FirstName = "Jane", LastName = "Doe" };
        var formFileMock = new Mock<Microsoft.AspNetCore.Http.IFormFile>();
        formFileMock.Setup(f => f.Length).Returns(1024);
        formFileMock.Setup(f => f.FileName).Returns("avatar.jpg");
        formFileMock.Setup(f => f.ContentType).Returns("image/jpeg");

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        _storageServiceMock.Setup(s => s.UploadBlobAsync(
            formFileMock.Object,
            BlobType.PROFILE,
            It.IsAny<string>(),
            It.IsAny<string>()))
            .ReturnsAsync("https://storage.blob.core.windows.net/profile/avatar.jpg");

        _userRepositoryMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(x => x.Map<UserResponse>(user))
            .Returns(new UserResponse { Id = 1, ProfileImageUrl = "https://storage.blob.core.windows.net/profile/avatar.jpg" });

        // Act
        var result = await _userService.UpdateProfileImageAsync(formFileMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.ProfileImageUrl.Should().Be("https://storage.blob.core.windows.net/profile/avatar.jpg");
        _userRepositoryMock.Verify(x => x.UpdateUserAsync(It.Is<User>(u =>
            u.ProfileImageUrl == "https://storage.blob.core.windows.net/profile/avatar.jpg"
        )), Times.Once);
    }

    [Test]
    public async Task RemoveProfileImageAsync_ShouldSetNullAndPersist()
    {
        // Arrange
        var user = new User { Id = 1, FirstName = "Jane", ProfileImageUrl = "https://storage.blob.core.windows.net/profile/old.jpg" };

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(x => x.Map<UserResponse>(user))
            .Returns(new UserResponse { Id = 1, ProfileImageUrl = null });

        // Act
        var result = await _userService.RemoveProfileImageAsync();

        // Assert
        result.Should().NotBeNull();
        result.ProfileImageUrl.Should().BeNull();
        _userRepositoryMock.Verify(x => x.UpdateUserAsync(It.Is<User>(u => u.ProfileImageUrl == null)), Times.Once);
    }

    [Test]
    public async Task GetCurrentUserProfileAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(new User { Id = 99 });

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(99))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _userService.GetCurrentUserProfileAsync();

        // Assert
        await act.Should().ThrowAsync<Core.Exceptions.NotFoundException>()
            .WithMessage("User not found");
    }

    [Test]
    public async Task UpdateProfileImageAsync_ShouldThrowFailedOperationException_WhenFileIsNull()
    {
        // Arrange
        var user = new User { Id = 1 };
        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = async () => await _userService.UpdateProfileImageAsync(null!);

        // Assert
        await act.Should().ThrowAsync<Core.Exceptions.FailedOperationException>()
            .WithMessage("Please provide a valid image file.");
    }

    [Test]
    public async Task UpdateProfileImageAsync_ShouldThrowFailedOperationException_WhenFileIsOversized()
    {
        // Arrange
        var user = new User { Id = 1 };
        var formFileMock = new Mock<Microsoft.AspNetCore.Http.IFormFile>();
        formFileMock.Setup(f => f.Length).Returns(6 * 1024 * 1024); // 6MB

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = async () => await _userService.UpdateProfileImageAsync(formFileMock.Object);

        // Assert
        await act.Should().ThrowAsync<Core.Exceptions.FailedOperationException>()
            .WithMessage("Image file size cannot exceed 5MB.");
    }

    [Test]
    public async Task UpdateProfileImageAsync_ShouldThrowFailedOperationException_WhenFileHasInvalidExtension()
    {
        // Arrange
        var user = new User { Id = 1 };
        var formFileMock = new Mock<Microsoft.AspNetCore.Http.IFormFile>();
        formFileMock.Setup(f => f.Length).Returns(1024);
        formFileMock.Setup(f => f.FileName).Returns("script.exe");
        formFileMock.Setup(f => f.ContentType).Returns("application/x-msdownload");

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = async () => await _userService.UpdateProfileImageAsync(formFileMock.Object);

        // Assert
        await act.Should().ThrowAsync<Core.Exceptions.FailedOperationException>()
            .WithMessage("Invalid image file format. Only JPEG, PNG, WEBP, and GIF are allowed.");
    }

    [Test]
    public async Task UpdateUserAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotOwnerAndNotAdmin()
    {
        // Arrange
        var request = new UserUpdateRequest { Id = 2 };

        _contextServiceMock.Setup(x => x.GetUser())
            .ReturnsAsync(new User { Id = 1, Role = UserRole.CUSTOMER });

        // Act
        Func<Task> act = async () => await _userService.UpdateUserAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You are not authorized to update this profile.");
    }

    #endregion
}