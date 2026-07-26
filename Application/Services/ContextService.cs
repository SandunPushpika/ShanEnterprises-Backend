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
        var claims = _httpContextAccessor.HttpContext?.User;
        
        var userId = claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? claims?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? claims?.FindFirst("sub")?.Value;

        var userEmail = claims?.FindFirst(ClaimTypes.Email)?.Value
                     ?? claims?.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                     ?? claims?.FindFirst("email")?.Value;

        var role = claims?.FindFirst(ClaimTypes.Role)?.Value
                ?? claims?.FindFirst("role")?.Value;

        var name = claims?.FindFirst("name")?.Value ?? "";

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid or missing user identity in token.");

        var nameParts = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.Length > 0 ? nameParts[0] : (userEmail?.Split('@')[0] ?? "User");
        var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

        var userRole = Enum.TryParse<UserRole>(role, true, out var parsedRole) ? parsedRole : UserRole.CUSTOMER;
        
        return Task.FromResult(new User()
        {
            Id = int.Parse(userId),
            Email = userEmail ?? "",
            Role = userRole,
            FirstName = firstName,
            LastName = lastName,
        });
    }
}