using Core.DTOs.Request.Auth;
using Core.Entities;
using Core.Enums;
using Core.Exceptions.Auth;
using Core.Helpers;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;

namespace Test.Application.AuthServiceTests;

public partial class AuthServiceTests
{
    #region RegisterUser

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
            Password = "123456"
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
    }

    #endregion

    #region LoginUser

    [Test]
    public async Task LoginUser_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "notfound@test.com",
            Password = "123"
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email.ToLower()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _authService.LoginUser(request);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidCredentialsException>();
    }

    [Test]
    public async Task LoginUser_ShouldReturnTokens_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "123456"
        };

        var user = new User
        {
            Id = 1,
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            Role = UserRole.ADMIN
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email.ToLower()))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginUser(request);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.UserRole.Should().Be(UserRole.ADMIN);
    }

    #endregion

    #region RefreshToken

    [Test]
    public async Task RefreshToken_ShouldThrowException_WhenTokenIsInvalid()
    {
        // Arrange
        string token = "invalid_token";

        // You can mock static JwtHelper using wrapper in real projects
        // Here we assume it returns null internally

        // Act
        Func<Task> act = async () => await _authService.RefreshToken(token);

        // Assert
        await act.Should()
            .ThrowAsync<SecurityTokenMalformedException>();
    }

    #endregion
}