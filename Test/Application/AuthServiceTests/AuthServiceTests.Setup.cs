using NUnit.Framework;
using Application.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using Core.Helpers;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace Test.Application.AuthServiceTests;

[TestFixture]
public partial class AuthServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IOptions<AppSettings>> _appSettingsMock;
    private Mock<IEmailService> _emailServiceMock;
    private Mock<IVerificationCodeRepository> _verificationCodeRepositoryMock;

    private AuthService _authService;

    private AppSettings _appSettings;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _appSettingsMock = new Mock<IOptions<AppSettings>>();
        _emailServiceMock = new Mock<IEmailService>();
        _verificationCodeRepositoryMock = new Mock<IVerificationCodeRepository>();

        _appSettings = new AppSettings
        {
            JwtSettings = new JwtSettings
            {
                SecurityKey = "Kp9xZ7vT2mQ8sD6fR1nY5hJ3cL8wA4bE9uT7xP2mK6sD1nF5hJ8cV3bR9yX0aQ",
                Issuer = "test",
                Audience = "test"
            }
        };

        _appSettingsMock.Setup(x => x.Value).Returns(_appSettings);

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _verificationCodeRepositoryMock.Object,
            _emailServiceMock.Object,
            _mapperMock.Object,
            _appSettingsMock.Object
        );
    }
}