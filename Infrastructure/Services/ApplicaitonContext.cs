using System.Security.Claims;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class ApplicationContext : IApplicationContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ApplicationContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;       
    }
    
    public User GetUser()
    {
        var userClaims = _httpContextAccessor?.HttpContext?.User.Claims?.ToList();
        if (userClaims == null)
            return null;
        
        return new User()
        {
            Id = Convert.ToInt64(userClaims.Find(x => x.Type == ClaimTypes.NameIdentifier)?.Value),
            Email = userClaims.Find(x => x.Type == ClaimTypes.Email)?.Value,
        };
        
    }
}