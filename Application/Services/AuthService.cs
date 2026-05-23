using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response.Auth;
using Core.Entities;
using Core.Exceptions.Auth;
using Core.Helpers;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly AppSettings _appSettings;

    public AuthService(IUserRepository userRepository, IMapper mapper, IOptions<AppSettings> appSettings)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _appSettings = appSettings.Value;
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

    public async Task<LoginResponse> LoginUser(LoginRequest request)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
        if (existingUser == null)
            throw new InvalidCredentialsException();
        
        if(!PasswordHasher.VerifyPassword(request.Password, existingUser.PasswordHash))
            throw new InvalidCredentialsException();

        return GetLoginResponse(existingUser);
    }

    public async Task<LoginResponse> RefreshToken(string refreshToken)
    {
        var userId = JwtHelper.GetUserIdFromToken(refreshToken, _appSettings.JwtSettings);
        if(userId == null)
            throw new UnauthorizedUserException();

        var user = await _userRepository.GetUserByIdAsync(int.Parse(userId));
        if(user == null)
            throw new UnauthorizedUserException();
        
        return GetLoginResponse(user);
    }

    private LoginResponse GetLoginResponse(User user)
    {
        return new LoginResponse()
        {
            AccessToken = JwtHelper.GenerateToken(user, _appSettings.JwtSettings, 30),
            RefreshToken = JwtHelper.GenerateToken(user, _appSettings.JwtSettings, 120, false),
            UserRole = user.Role
        };
    }
}