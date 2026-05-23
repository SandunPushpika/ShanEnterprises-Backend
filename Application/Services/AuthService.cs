using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response.Auth;
using Core.Entities;
using Core.Exceptions.Auth;
using Core.Helpers;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AuthService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public async Task RegisterUser(CreateUserRequest request)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
        if (existingUser != null)
            throw new UserAlreadyExistsException(request.Email);
        
        var user = _mapper.Map<User>(request);
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = PasswordHasher.HashPassword(request.Password);
        user.EmailVerified = false;

        await _userRepository.AddUserAsync(user);
    }

    public Task<LoginResponse> LoginUser(LoginRequest request)
    {
        throw new NotImplementedException();
    }
}