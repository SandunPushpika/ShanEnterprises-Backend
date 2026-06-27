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
    public async Task VerifyCode_ShouldThrowException_WhenCodeDoesNotExist()
    {
        // Arrange
        string code = "nonexistent";
        _verificationCodeRepositoryMock
            .Setup(x => x.GetVerificationCodeByCode(code))
            .ReturnsAsync((VerificationCodes?)null);

        // Act
        Func<Task> act = async () => await _authService.VerifyCode(code);

        // Assert
        await act.Should()
            .ThrowAsync<VerificationCodeException>()
            .WithMessage("Invalid code");
    }

    [Test]
    public async Task VerifyCode_ShouldThrowException_WhenCodeIsAlreadyUsed()
    {
        // Arrange
        string code = "123456";
        var verificationCode = new VerificationCodes
        {
            VerificationCode = code,
            IsUsed = true,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        _verificationCodeRepositoryMock
            .Setup(x => x.GetVerificationCodeByCode(code))
            .ReturnsAsync(verificationCode);

        // Act
        Func<Task> act = async () => await _authService.VerifyCode(code);

        // Assert
        await act.Should()
            .ThrowAsync<VerificationCodeException>()
            .WithMessage("Code Expired");
    }

    [Test]
    public async Task VerifyCode_ShouldThrowException_WhenCodeIsExpired()
    {
        // Arrange
        string code = "123456";
        var verificationCode = new VerificationCodes
        {
            VerificationCode = code,
            IsUsed = false,
            ExpiresAt = DateTime.UtcNow.AddMinutes(-5)
        };

        _verificationCodeRepositoryMock
            .Setup(x => x.GetVerificationCodeByCode(code))
            .ReturnsAsync(verificationCode);

        // Act
        Func<Task> act = async () => await _authService.VerifyCode(code);

        // Assert
        await act.Should()
            .ThrowAsync<VerificationCodeException>()
            .WithMessage("Code Expired");
    }

    [Test]
    public async Task VerifyCode_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        string code = "123456";
        var verificationCode = new VerificationCodes
        {
            VerificationCode = code,
            IsUsed = false,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            UserId = 1
        };

        _verificationCodeRepositoryMock
            .Setup(x => x.GetVerificationCodeByCode(code))
            .ReturnsAsync(verificationCode);

        _userRepositoryMock
            .Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _authService.VerifyCode(code);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Test]
    public async Task VerifyCode_ShouldVerifyEmailAndSaveUser_WhenCodeIsValid()
    {
        // Arrange
        string code = "123456";
        var verificationCode = new VerificationCodes
        {
            VerificationCode = code,
            IsUsed = false,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            UserId = 1
        };

        var user = new User
        {
            Id = 1,
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        _verificationCodeRepositoryMock
            .Setup(x => x.GetVerificationCodeByCode(code))
            .ReturnsAsync(verificationCode);

        _userRepositoryMock
            .Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        await _authService.VerifyCode(code);

        // Assert
        user.EmailVerified.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.UpdateUserAsync(It.Is<User>(u => u.Id == 1 && u.EmailVerified == true)), Times.Once);
    }
}
