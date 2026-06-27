using System.Security.Claims;
using Core.Entities;
using Core.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Test.Application.ContextServiceTests;

public partial class ContextServiceTests
{
    [Test]
    public async Task GetUser_ShouldThrowUnauthorizedAccessException_WhenHttpContextIsNull()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        Func<Task> act = async () => await _contextService.GetUser();

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task GetUser_ShouldThrowUnauthorizedAccessException_WhenAnyRequiredClaimIsMissing()
    {
        // Arrange: Missing email claim
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "ADMIN")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(x => x.User).Returns(principal);
        
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        // Act
        Func<Task> act = async () => await _contextService.GetUser();

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task GetUser_ShouldReturnUser_WhenAllRequiredClaimsArePresent()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "42"),
            new Claim(ClaimTypes.Email, "driver@test.com"),
            new Claim(ClaimTypes.Role, "DRIVER")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(x => x.User).Returns(principal);
        
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        // Act
        var result = await _contextService.GetUser();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        result.Email.Should().Be("driver@test.com");
        result.Role.Should().Be(UserRole.DRIVER);
    }
}
