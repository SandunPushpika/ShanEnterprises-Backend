using Core.Entities;
using Core.Exceptions;
using Core.Exceptions.Auth;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Test.Application.AuthServiceTests;

public partial class AuthServiceTests
{
    [Test]
    public async Task ResendVerificationCode_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        string email = "notfound@test.com";
        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(email.ToLower()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _authService.ResendVerificationCode(email);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Test]
    public async Task ResendVerificationCode_ShouldThrowException_WhenUserAlreadyVerified()
    {
        // Arrange
        string email = "verified@test.com";
        var user = new User { Id = 1, Email = email, EmailVerified = true };

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(email.ToLower()))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = async () => await _authService.ResendVerificationCode(email);

        // Assert
        await act.Should()
            .ThrowAsync<VerificationCodeException>()
            .WithMessage("Email already Verified");
    }

    [Test]
    public async Task ResendVerificationCode_ShouldSendVerificationEmail_WhenUserIsNotVerified()
    {
        // Arrange
        string email = "unverified@test.com";
        var user = new User { Id = 1, Email = email, EmailVerified = false, FirstName = "John" };

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(email.ToLower()))
            .ReturnsAsync(user);

        // Act
        await _authService.ResendVerificationCode(email);

        // Assert
        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<Core.Helpers.MailSettings>(), It.IsAny<Core.DTOs.Request.Other.EmailSendRequest>()), Times.Once);
        _verificationCodeRepositoryMock.Verify(x => x.AddVerificationCode(It.IsAny<VerificationCodes>()), Times.Once);
    }
}
