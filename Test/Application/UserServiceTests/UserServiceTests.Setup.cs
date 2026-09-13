using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using AutoMapper;
using Core.Helpers;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services;

[TestFixture]
public partial class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IContextService> _contextServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IStorageService> _storageServiceMock;
    private Mock<IOptions<AppSettings>> _appSettingsMock;
    private AppSettings _appSettings;

    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _contextServiceMock = new Mock<IContextService>();
        _mapperMock = new Mock<IMapper>();
        _storageServiceMock = new Mock<IStorageService>();
        _appSettingsMock = new Mock<IOptions<AppSettings>>();

        _appSettings = new AppSettings
        {
            BlobConnectionString = "UseDevelopmentStorage=true"
        };
        _appSettingsMock.Setup(x => x.Value).Returns(_appSettings);

        _userService = new UserService(
            _userRepositoryMock.Object,
            _contextServiceMock.Object,
            _mapperMock.Object,
            _storageServiceMock.Object,
            _appSettingsMock.Object
        );
    }
}