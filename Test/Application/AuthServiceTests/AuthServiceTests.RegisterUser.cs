using Core.DTOs.Request.Auth;
using Core.Entities;
using Core.Enums;
using Core.Exceptions.Auth;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Test.Application.AuthServiceTests;

public partial class AuthServiceTests
{
    [Test]
    public async Task RegisterUser_ShouldThrowException_WhenUserAlreadyExists()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "test@test.com",
            Password = "123456"
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email.ToLower()))
            .ReturnsAsync(new User());

        // Act
        Func<Task> act = async () => await _authService.RegisterUser(request);

        // Assert
        await act.Should()
            .ThrowAsync<UserAlreadyExistsException>();
    }

    [Test]
    public async Task RegisterUser_ShouldCreateUser_WhenEmailIsNew()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "new@test.com",
            Password = "123456",
            Role = UserRole.CUSTOMER
        };
        var user = new User()
        {
            Id = 1
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email.ToLower()))
            .ReturnsAsync((User?)null);

        _mapperMock
            .Setup(x => x.Map<User>(request))
            .Returns(new User { Email = request.Email });

        _userRepositoryMock
            .Setup(x => x.AddUserAsync(It.IsAny<User>()))
            .Returns(Task.FromResult(user));

        // Act
        await _authService.RegisterUser(request);

        // Assert
        _userRepositoryMock.Verify(x => x.AddUserAsync(It.Is<User>(u =>
            u.Email == request.Email &&
            u.EmailVerified == false &&
            u.PasswordHash != null
        )), Times.Once);
        
        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<Core.Helpers.MailSettings>(), It.IsAny<Core.DTOs.Request.Other.EmailSendRequest>()), Times.Once);
        _verificationCodeRepositoryMock.Verify(x => x.AddVerificationCode(It.IsAny<VerificationCodes>()), Times.Once);
    }

    [Test]
    public async Task RegisterUser_ShouldThrowException_WhenAdminRoleAndInvalidAdminKey()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "admin@test.com",
            Password = "123",
            Role = UserRole.ADMIN,
            KeyCode = "wrong-key"
        };

        _appSettings.AdminKeyCode = "correct-key";

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email.ToLower()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _authService.RegisterUser(request);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid Admin Key!");
    }

    [Test]
    public async Task RegisterUser_ShouldCreateAdminUser_WhenAdminRoleAndValidAdminKey()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "admin@test.com",
            Password = "123",
            Role = UserRole.ADMIN,
            KeyCode = "correct-key"
        };
        var user = new User()
        {
            Id = 2,
            Role = UserRole.ADMIN
        };

        _appSettings.AdminKeyCode = "correct-key";

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email.ToLower()))
            .ReturnsAsync((User?)null);

        _mapperMock
            .Setup(x => x.Map<User>(request))
            .Returns(new User { Email = request.Email, Role = request.Role });

        _userRepositoryMock
            .Setup(x => x.AddUserAsync(It.IsAny<User>()))
            .ReturnsAsync(user);

        // Act
        await _authService.RegisterUser(request);

        // Assert
        _userRepositoryMock.Verify(x => x.AddUserAsync(It.Is<User>(u =>
            u.Email == request.Email &&
            u.EmailVerified == true
        )), Times.Once);
        
        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<Core.Helpers.MailSettings>(), It.IsAny<Core.DTOs.Request.Other.EmailSendRequest>()), Times.Never);
        _verificationCodeRepositoryMock.Verify(x => x.AddVerificationCode(It.IsAny<VerificationCodes>()), Times.Never);
    }

    [Test]
    public async Task RegisterUser_ShouldCreateUserWithoutSendingVerificationEmail_WhenSocialMediaRequest()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "social@test.com",
            Password = "123",
            Role = UserRole.CUSTOMER
        };
        var user = new User()
        {
            Id = 3,
            Role = UserRole.CUSTOMER
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email.ToLower()))
            .ReturnsAsync((User?)null);

        _mapperMock
            .Setup(x => x.Map<User>(request))
            .Returns(new User { Email = request.Email, Role = request.Role });

        _userRepositoryMock
            .Setup(x => x.AddUserAsync(It.IsAny<User>()))
            .ReturnsAsync(user);

        // Act
        await _authService.RegisterUser(request, socialMediaRequest: true);

        // Assert
        _userRepositoryMock.Verify(x => x.AddUserAsync(It.Is<User>(u =>
            u.Email == request.Email &&
            u.EmailVerified == true
        )), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<Core.Helpers.MailSettings>(), It.IsAny<Core.DTOs.Request.Other.EmailSendRequest>()), Times.Never);
        _verificationCodeRepositoryMock.Verify(x => x.AddVerificationCode(It.IsAny<VerificationCodes>()), Times.Never);
    }
}
