using Core.Entities;
using Core.Exceptions;
using Core.Exceptions.Auth;
using Core.Helpers;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;

namespace Test.Application.AuthServiceTests;

public partial class AuthServiceTests
{
    [Test]
    public async Task RefreshToken_ShouldThrowException_WhenTokenIsInvalid()
    {
        // Arrange
        string token = "invalid_token";

        // Act
        Func<Task> act = async () => await _authService.RefreshToken(token);

        // Assert
        await act.Should()
            .ThrowAsync<SecurityTokenMalformedException>();
    }

    [Test]
    public async Task RefreshToken_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var user = new User { Id = 5, Email = "nonexistent@test.com" };
        var validToken = JwtHelper.GenerateToken(user, _appSettings.JwtSettings, 120, false);

        _userRepositoryMock
            .Setup(x => x.GetUserByIdAsync(5))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _authService.RefreshToken(validToken);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedUserException>();
    }

    [Test]
    public async Task RefreshToken_ShouldReturnNewTokens_WhenTokenAndUserAreValid()
    {
        // Arrange
        var user = new User { Id = 5, Email = "user@test.com", Role = Core.Enums.UserRole.DRIVER };
        var validToken = JwtHelper.GenerateToken(user, _appSettings.JwtSettings, 120, false);

        _userRepositoryMock
            .Setup(x => x.GetUserByIdAsync(5))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.RefreshToken(validToken);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.UserRole.Should().Be(Core.Enums.UserRole.DRIVER);
    }
}
