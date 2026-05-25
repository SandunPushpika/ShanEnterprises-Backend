using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using AutoMapper;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services;

[TestFixture]
public partial class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IContextService> _contextServiceMock;
    private Mock<IMapper> _mapperMock;

    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _contextServiceMock = new Mock<IContextService>();
        _mapperMock = new Mock<IMapper>();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _contextServiceMock.Object,
            _mapperMock.Object
        );
    }
}