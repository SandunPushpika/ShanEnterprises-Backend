using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Application.Services;

namespace Test.Application.ContextServiceTests;

[TestFixture]
public partial class ContextServiceTests
{
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private ContextService _contextService;

    [SetUp]
    public void Setup()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _contextService = new ContextService(_httpContextAccessorMock.Object);
    }
}
