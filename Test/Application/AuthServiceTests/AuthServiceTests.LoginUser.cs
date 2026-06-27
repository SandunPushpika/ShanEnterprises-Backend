using Core.DTOs.Request.Auth;
using Core.Entities;
using Core.Enums;
using Core.Exceptions.Auth;
using Core.Helpers;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Test.Application.AuthServiceTests;

public partial class AuthServiceTests
{
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
    public async Task LoginUser_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "wrongpassword"
        };
        var user = new User
        {
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword("correctpassword")
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email.ToLower()))
            .ReturnsAsync(user);

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
}
