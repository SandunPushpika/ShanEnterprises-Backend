using Core.DTOs.Request.Auth;
using Core.DTOs.Response.Auth;
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
    public void GetOAuthUrl_ShouldReturnAuthorizeUrl()
    {
        // Arrange
        var expectedUrl = "https://google.com/oauth";
        _oauthServiceMock.Setup(x => x.GetAuthorizeUrl()).Returns(expectedUrl);

        // Act
        var result = _authService.GetOAuthUrl();

        // Assert
        result.Should().Be(expectedUrl);
    }

    [Test]
    public async Task LoginViaSocialMedia_ShouldThrowException_WhenUserInfoIsNull()
    {
        // Arrange
        var code = "auth-code";
        _oauthServiceMock
            .Setup(x => x.ExchangeCodeForTokenAsync(code))
            .ReturnsAsync(new TokenResponse { AccessToken = "access-token" });

        _oauthServiceMock
            .Setup(x => x.GetUserInfoAsync("access-token"))
            .ReturnsAsync((UserInfoResponse?)null);

        // Act
        Func<Task> act = async () => await _authService.LoginViaSocialMedia(code);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedUserException>();
    }

    [Test]
    public async Task LoginViaSocialMedia_ShouldRegisterNewUserAndReturnTokens_WhenUserDoesNotExist()
    {
        // Arrange
        var code = "auth-code";
        var userInfo = new UserInfoResponse
        {
            Email = "social@test.com",
            FullName = "John Doe"
        };

        _oauthServiceMock
            .Setup(x => x.ExchangeCodeForTokenAsync(code))
            .ReturnsAsync(new TokenResponse { AccessToken = "access-token" });

        _oauthServiceMock
            .Setup(x => x.GetUserInfoAsync("access-token"))
            .ReturnsAsync(userInfo);

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(userInfo.Email))
            .ReturnsAsync((User?)null);

        _mapperMock
            .Setup(x => x.Map<User>(It.IsAny<CreateUserRequest>()))
            .Returns(new User { Email = userInfo.Email, Role = UserRole.CUSTOMER });

        _userRepositoryMock
            .Setup(x => x.AddUserAsync(It.IsAny<User>()))
            .ReturnsAsync(new User { Id = 10, Email = userInfo.Email, Role = UserRole.CUSTOMER, Status = UserStatus.ACTIVE, EmailVerified = true });

        // Act
        var result = await _authService.LoginViaSocialMedia(code);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.UserRole.Should().Be(UserRole.CUSTOMER);
    }

    [Test]
    public async Task LoginViaSocialMedia_ShouldUpdateStatusAndReturnTokens_WhenUserExistsButIsInactiveOrNotVerified()
    {
        // Arrange
        var code = "auth-code";
        var userInfo = new UserInfoResponse
        {
            Email = "social@test.com",
            FullName = "John Doe"
        };
        var existingUser = new User
        {
            Id = 10,
            Email = userInfo.Email,
            Role = UserRole.CUSTOMER,
            Status = UserStatus.INACTIVE,
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        _oauthServiceMock
            .Setup(x => x.ExchangeCodeForTokenAsync(code))
            .ReturnsAsync(new TokenResponse { AccessToken = "access-token" });

        _oauthServiceMock
            .Setup(x => x.GetUserInfoAsync("access-token"))
            .ReturnsAsync(userInfo);

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(userInfo.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _authService.LoginViaSocialMedia(code);

        // Assert
        result.Should().NotBeNull();
        _userRepositoryMock.Verify(x => x.UpdateUserAsync(It.Is<User>(u =>
            u.Id == 10 &&
            u.Status == UserStatus.ACTIVE &&
            u.EmailVerified == true
        )), Times.Once);
    }
}
