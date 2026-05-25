using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Interfaces.Services;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class ContextService : IContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Task<User> GetUser()
    {
        var claims = _httpContextAccessor.HttpContext.User;
        
        var userId = claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = claims?.FindFirst(ClaimTypes.Email)?.Value;
        var role = claims?.FindFirst(ClaimTypes.Role)?.Value;

        if (userId == null || userEmail == null || role == null)
            throw new UnauthorizedAccessException();
        
        return Task.FromResult(new User()
        {
            Id = int.Parse(userId),
            Email = userEmail,
            Role = Enum.Parse<UserRole>(role)
        });
    }
}