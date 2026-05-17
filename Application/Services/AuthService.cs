using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response.Auth;
using Core.Entities;
using Core.Exceptions.Auth;
using Core.Helpers;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task RegisterUser(CreateUserRequest request)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
        if (existingUser != null)
            throw new UserAlreadyExistsException(request.Email);
        
        var user = new User()
        {
            Email = request.Email.ToLower(),
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow,
        };
        
        await _userRepository.AddUserAsync(user);
    }

    public Task<LoginResponse> LoginUser(LoginRequest request)
    {
        throw new NotImplementedException();
    }
}